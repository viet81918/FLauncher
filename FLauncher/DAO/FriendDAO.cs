using FLauncher.Model;


using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using FLauncher.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using FLauncher.Repositories;
using System.Windows;

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

        public List<Friend> GetFriendInvitationsForGamer(Gamer gamer)
        {
            if (gamer == null)
            {
                throw new ArgumentNullException(nameof(gamer), "Gamer cannot be null");
            }

            // Use ToList() instead of ToListAsync() for synchronous execution
            var invitations = _dbContext.Friends
                .Where(friend => friend.AcceptId == gamer.GamerId && friend.IsAccept == null)
                .ToList();  // This is now synchronous

            Debug.WriteLine($"Fetched {invitations.Count} invitations for gamer {gamer.GamerId}");

            return invitations;
        }



        public async Task InsertFriendRequest(Friend friendRequest)
        {
            _dbContext.Friends.Add(friendRequest);
            await _dbContext.SaveChangesAsync();  // Use async SaveChanges to avoid blocking the thread
        }

        public async Task<List<Friend>> GetFriendsForGamer(Gamer gamer)
        {
            // Use async query to fetch friends for the given gamer
            return await _dbContext.Friends
                .Where(f => f.RequestId == gamer.GamerId || f.AcceptId == gamer.GamerId)
                .ToListAsync(); // Using ToListAsync for asynchronous operation
        }
                                                                                                        

        public List<Friend> GetFriendsForAGamer(Gamer gamer)
        {
            // Synchronous query to fetch friends for the given gamer
            return _dbContext.Friends
                .Where(f => f.RequestId == gamer.GamerId || f.AcceptId == gamer.GamerId)
                .ToList(); // Using ToList for synchronous operation
        }


        public List<Gamer> GetFriendWithTheSameGame(Game game, Gamer gamer)
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

            // Truy vấn các gamer từ các gamerIds
            var friendsWithPurchasedGame = gamerDAO.GetGamersByIds(gamerIdsWithPurchasedGame);

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
