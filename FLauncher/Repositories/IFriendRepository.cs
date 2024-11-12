﻿using FLauncher.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public interface IFriendRepository
    {
        Task AddFriendRequest(Friend friendRequest);
        Task<Friend> GetFriendRequest(string requestId, string acceptId);
        Task UpdateFriendRequestStatus(string requestId, string acceptId, bool isAccepted);
 
        Task<List<Friend>> GetFriendsForGamer(Gamer gamer);
        Task<Friend> GetFriendship(string gamerId1, string gamerId2);
        List<Friend> GetFriendInvitationsforGamer(Gamer gamer);
        List<Gamer> GetFriendWithTheSameGame(Game game, Gamer gamer);

    }
}
