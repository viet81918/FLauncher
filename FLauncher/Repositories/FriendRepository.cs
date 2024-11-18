using FLauncher.DAO;
using FLauncher.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly FriendDAO _friendDAO;

        public FriendRepository()
        {
            _friendDAO = FriendDAO.Instance; // Assuming FriendDAO follows Singleton pattern
        }

        public async Task AddFriendRequest(Friend friendRequest)
        {
            await _friendDAO.InsertFriendRequest(friendRequest);
        }

        public async Task<Friend> GetFriendRequest(string requestId, string acceptId)
        {
            return await _friendDAO.FindFriendRequest(requestId, acceptId);
        }

        public async Task UpdateFriendRequestStatus(string requestId, string acceptId, bool isAccepted)
        {
            await _friendDAO.UpdateFriendRequestStatus(requestId, acceptId, isAccepted);
        }

        public async Task<List<Friend>> GetFriendInvitationsForGamer(Gamer gamer)
        {
            return await _friendDAO.GetFriendInvitationsForGamer(gamer);  // Call the synchronous DAO method
        }

        public List<Gamer> GetAllFriendByGamer(Gamer gamer)
        {
            return _friendDAO.GetAllFriendByGamer(gamer);
        }

        public async Task<List<Friend>> GetFriendsForGamer(Gamer gamer)
        {
            // Use the DAO method to retrieve the list of friends asynchronously
            return await _friendDAO.GetFriendsForGamer(gamer);
        }

        public async Task<Friend> GetFriendship(string gamerId1, string gamerId2)
        {
            return await _friendDAO.GetFriendship(gamerId1, gamerId2);
        }

        public async Task<IEnumerable<Gamer>> GetFriendWithTheSameGame(Game game, Gamer gamer)
        {
            return await FriendDAO.Instance.GetFriendWithTheSameGame(game, gamer);
        }

      
    }
}
