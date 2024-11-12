using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<Friend> GetFriendInvitationsforGamer(Gamer gamer)
        {
           
            return _dbContext.Friends
                .Where(friend => friend.AcceptId == gamer.GamerId)
                .ToList();
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




    }
}
