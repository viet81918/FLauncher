using FLauncher.Model;


using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using FLauncher.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

using MongoDB.Bson;

using FLauncher.Repositories;
using System.Windows;


namespace FLauncher.DAO
{
    public class FriendDAO : SingletonBase<FriendDAO>
    {
       
        private readonly FlauncherDbContext _dbContext;

        private readonly IMongoCollection<BsonDocument> _friendCollection;
        private readonly GamerDAO _gamerDAO;


        public FriendDAO()
        {
            
            var connectionString = "mongodb://localhost:27017/";   
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("FPT");
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
            _friendCollection = database.GetCollection<BsonDocument>("Friends");
            _gamerDAO = GamerDAO.Instance;
        }
        public List<Gamer> GetAllFriendByGamer(Gamer gamer)
        {
            
            // Lấy danh sách các Friend Id mà người chơi (gamer) có trong cả RequestId hoặc AcceptId và IsAccept = true
            var friendIds = _dbContext.Friends
                                      .Where(f => (f.RequestId == gamer.GamerId || f.AcceptId == gamer.GamerId) && f.IsAccept == true)
                                      .Select(f => f.RequestId == gamer.GamerId ? f.AcceptId : f.RequestId) // Chọn ID bạn bè từ cả hai trường
                                      .ToList();

            if (friendIds.Count == 0)
            {
                MessageBox.Show("Không có bạn bè.");
                return new List<Gamer>();
            }

            // Lấy thông tin các Gamer từ bảng Gamers dựa trên danh sách các Friend ID
            var friends = _dbContext.Gamers
                                    .Where(g => friendIds.Contains(g.GamerId))
                                    .ToList();

            return friends;
        }


        public async Task<List<Friend>> GetFriendInvitationsForGamer(Gamer gamer)
{
    if (gamer == null)
    {
        throw new ArgumentNullException(nameof(gamer), "Gamer cannot be null");
    }

    // Use ToList() instead of ToListAsync() for synchronous execution
    var invitations = await _dbContext.Friends
        .Where(friend => friend.AcceptId == gamer.GamerId && friend.IsAccept == null)
        .ToListAsync();  // This is now synchronous

            Debug.WriteLine($"Fetched {invitations.Count} invitations for gamer {gamer.GamerId}");

            return invitations;
        }



        public async Task InsertFriendRequest(Friend friendRequest)
        {
            _dbContext.Friends.Add(friendRequest);
            await _dbContext.SaveChangesAsync(); 
        }

        public async Task<List<Friend>> GetFriendsForGamer(Gamer gamer)
        {
            // Use async query to fetch friends for the given gamer
            return await _dbContext.Friends
                .Where(f => f.RequestId == gamer.GamerId || f.AcceptId == gamer.GamerId && f.IsAccept==true )
                .ToListAsync(); // Using ToListAsync for asynchronous operation
        }
                                                                                                        

        public List<Friend> GetFriendsForAGamer(Gamer gamer)
        {
            // Synchronous query to fetch friends for the given gamer
            return _dbContext.Friends
                .Where(f => f.RequestId == gamer.GamerId || f.AcceptId == gamer.GamerId)
                .ToList(); // Using ToList for synchronous operation
        }


        public async Task<IEnumerable<Gamer>> GetFriendWithTheSameGame(Game game, Gamer gamer)
        {
            var gamerDAO = new GamerDAO();

            // Lấy danh sách bạn bè của gamer
            var friends = GetFriendsForAGamer(gamer);

            // Danh sách người chơi có cùng game
            var friendsWithSameGame = new List<Gamer>();

            // 1. Lấy danh sách bạn bè theo ID
            var friendIds = friends.Select(f => f.RequestId == gamer.GamerId ? f.AcceptId : f.RequestId).ToList();

            // 2. Lấy danh sách các Bill mà những người bạn này đã mua game từ bảng Bills
            var purchasedGameBills = _dbContext.Bills
                                                .Where(b => friendIds.Contains(b.GamerId) && b.GameId == game.GameID)
                                                .ToList();

            // 3. Lấy danh sách gamer từ các IDs đã mua game
            var gamerIdsWithPurchasedGame = purchasedGameBills
                .Select(b => b.GamerId)
                .Distinct()
                .ToList();

            // Truy vấn các gamer từ các gamerIds (dùng await)
            var friendsWithPurchasedGame =  gamerDAO.GetGamersByIds(gamerIdsWithPurchasedGame);

            // Trả về danh sách bạn bè đã mua game
            return await friendsWithPurchasedGame;
        }





        public async Task<Friend> FindFriendRequest(string requestId, string acceptId)
        {
            return await _dbContext.Friends
                .Where(friend => friend.RequestId == requestId && friend.AcceptId == acceptId && friend.IsAccept == null)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateFriendRequestStatus(string requestId, string acceptId, bool isAccepted)
        {
            var friendRequest = await _dbContext.Friends
                .FirstOrDefaultAsync(friend => friend.RequestId == requestId && friend.AcceptId == acceptId);

            if (friendRequest == null)
            {
                throw new Exception("Friend request not found.");
            }

            friendRequest.IsAccept = isAccepted;

            var result = await _dbContext.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("Friend request not updated.");
            }
        }



        public async Task<Friend> GetFriendship(string gamerId1, string gamerId2)
        {
            return await _dbContext.Friends
                .Where(friend =>
                    (friend.RequestId == gamerId1 && friend.AcceptId == gamerId2 ||
                     friend.RequestId == gamerId2 && friend.AcceptId == gamerId1) &&
                    friend.IsAccept == true)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Friend>> GetFriendshipsForGamer(string gamerId)
        {
            var requestIdFilter = Builders<BsonDocument>.Filter.Eq("Request_id", gamerId);
            var acceptIdFilter = Builders<BsonDocument>.Filter.Eq("Accept_id", gamerId);
            var isAcceptFilter = Builders<BsonDocument>.Filter.Eq("isAccept", true);

            var filter = Builders<BsonDocument>.Filter.And(
                isAcceptFilter,
                Builders<BsonDocument>.Filter.Or(requestIdFilter, acceptIdFilter)
            );

            // Log the filter conditions to see the structure clearly
            Debug.WriteLine($"RequestId filter: {requestIdFilter}");
            Debug.WriteLine($"AcceptId filter: {acceptIdFilter}");
            Debug.WriteLine($"IsAccept filter: {isAcceptFilter}");
            Debug.WriteLine($"Complete Filter: {filter}");

            var friendsCursor = await _friendCollection.Find(filter).ToListAsync();
            Debug.WriteLine($"Friendships retrieved for GamerId {gamerId}: {friendsCursor.Count}");


            var friendships = friendsCursor.Select(doc => new Friend
            {
                RequestId = doc["Request_id"].AsString,
                AcceptId = doc["Accept_id"].AsString,
                IsAccept = doc["isAccept"].AsBoolean
            }).ToList();

            // Log the number of friendships found
            Debug.WriteLine($"Number of friendships: {friendships.Count}");

            return friendships;
        }



        public async Task<IEnumerable<Gamer>> GetListFriendForGamer(string gamerId)
        {
            Debug.WriteLine($"GetListFriendForGamer called with gamerId: {gamerId}");

            // Retrieve friendships where the gamer is either the requester or the accepter
            var friendships = await GetFriendshipsForGamer(gamerId);
            

            // Extract unique friend IDs (other than the gamerId itself)
            var friendIds = friendships
                .Select(friend => friend.RequestId == gamerId ? friend.AcceptId : friend.RequestId)
                .Distinct()
                .ToList();
            Debug.WriteLine($"Friend IDs: {string.Join(", ", friendIds)}");

            // Fetch all gamers in a single database call
            var friends = await _gamerDAO.GetGamersByIds(friendIds);
            Debug.WriteLine($"Gamers fetched. Count: {friends.Count}");
            return friends;
        }




    }
}
