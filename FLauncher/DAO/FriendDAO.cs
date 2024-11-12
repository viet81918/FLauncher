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
        private readonly MongoDbContext _mongoDbContext;

        public FriendDAO()
        {
            _mongoDbContext = new MongoDbContext();
        }

        public List<Friend> GetFriendInvitationsForGamer(Gamer gamer)
        {
            if (gamer == null)
            {
                throw new ArgumentNullException(nameof(gamer), "Gamer cannot be null");
            }

            var invitations = _mongoDbContext.Friends
                .Find(friend => friend.AcceptId == gamer.GamerId && friend.IsAccept == null)
                .ToList();

            Debug.WriteLine($"Fetched {invitations.Count} invitations for gamer {gamer.GamerId}");

            return invitations;
        }

        public async Task InsertFriendRequest(Friend friendRequest)
        {
            await _mongoDbContext.Friends.InsertOneAsync(friendRequest);
        }
        public List<Friend> GetFriendsForGamer(Gamer gamer)
        {
            // Lấy tất cả các mối quan hệ bạn bè mà người chơi có thể là RequestId hoặc AcceptId
            var friends = _dbContext.Friends.Where(f => f.RequestId == gamer.GamerId || f.AcceptId == gamer.GamerId)
                          .ToList();
            return friends;
        }

        public List<Gamer> GetFriendWithTheSameGame(Game game, Gamer gamer)
        {
            var gamerDAO = new GamerDAO();

            // Lấy danh sách bạn bè của gamer
            var friends = GetFriendsForGamer(gamer);

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
            return await _mongoDbContext.Friends
                .Find(friend => friend.RequestId == requestId && friend.AcceptId == acceptId && friend.IsAccept == null)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateFriendRequestStatus(string requestId, string acceptId, bool isAccepted)
        {
            var filter = Builders<Friend>.Filter.Eq(f => f.RequestId, requestId) &
                         Builders<Friend>.Filter.Eq(f => f.AcceptId, acceptId);

            var update = Builders<Friend>.Update.Set(f => f.IsAccept, isAccepted);

            var result = await _mongoDbContext.Friends.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
            {
                throw new Exception("Friend request not found or already updated.");
            }
        }
        public async Task<List<Friend>> GetFriendsForGamer(Gamer gamer)
        {
            if (gamer == null)
            {
                throw new ArgumentNullException(nameof(gamer), "Gamer cannot be null");
            }

            // Query the database for friends where either RequestId or AcceptId matches the gamer
            // and the IsAccept flag is true
            return await _mongoDbContext.Friends
                .Find(friend =>
                    (friend.RequestId == gamer.GamerId || friend.AcceptId == gamer.GamerId) &&
                    friend.IsAccept == true)
                .ToListAsync();
        }

        public async Task<Friend> GetFriendship(string gamerId1, string gamerId2)
        {
            return await _mongoDbContext.Friends
                .Find(friend =>
                    (friend.RequestId == gamerId1 && friend.AcceptId == gamerId2 ||
                     friend.RequestId == gamerId2 && friend.AcceptId == gamerId1) &&
                    friend.IsAccept == true)
                .FirstOrDefaultAsync();
        }
    }
}
