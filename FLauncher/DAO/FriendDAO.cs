using FLauncher.Model;


using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using FLauncher.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FLauncher.DAO
{
    public class FriendDAO : SingletonBase<FriendDAO>
    {
       
        private readonly FlauncherDbContext _dbContext;

        public FriendDAO()
        {
            
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
        }

        public async Task<IEnumerable<Friend>> GetFriendInvitationsForGamer(Gamer gamer)
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
            var friendsWithPurchasedGame = await gamerDAO.GetGamersByIds(gamerIdsWithPurchasedGame);

            // Trả về danh sách bạn bè đã mua game
            return friendsWithPurchasedGame;
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
    }
}
