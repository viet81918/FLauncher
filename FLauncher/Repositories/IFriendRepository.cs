using FLauncher.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public interface IFriendRepository
    {
        // Add a friend request
        Task AddFriendRequest(Friend friendRequest);

        // Get a friend request by requestId and acceptId
        Task<Friend> GetFriendRequest(string requestId, string acceptId);

        // Update the status of a friend request (accepted or rejected)
        Task UpdateFriendRequestStatus(string requestId, string acceptId, bool isAccepted);

        // Get pending friend invitations for a gamer
        List<Friend> GetFriendInvitationsForGamer(Gamer gamer);

        // Get all friends for a gamer
        Task<List<Friend>> GetFriendsForGamer(Gamer gamer);

        // Get a specific friendship (if exists) between two gamers
        Task<Friend> GetFriendship(string gamerId1, string gamerId2);

        // Get friends who have the same game as the given gamer
        List<Gamer> GetFriendWithTheSameGame(Game game, Gamer gamer);

    }
}
