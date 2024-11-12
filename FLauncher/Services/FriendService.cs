using FLauncher.DAO;
using FLauncher.Model;
using FLauncher.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FLauncher.Services
{
    public class FriendService
    {
        private readonly FriendRepository _friendRepository;
        private readonly GamerRepository _gamerRepository;

        public FriendService(FriendRepository friendRepository, GamerRepository gamerRepository)
        {
            _friendRepository = friendRepository;
            _gamerRepository = gamerRepository;
        }

        public async Task<bool> AreGamersAlreadyFriends(string gamerId1, string gamerId2)
        {
            var existingFriendship = await _friendRepository.GetFriendship(gamerId1, gamerId2);

            return existingFriendship != null && existingFriendship.IsAccept == true;
        }

        public async Task<bool> SendFriendRequest(string requestId, string acceptId)
        {
            var existingRequest = await _friendRepository.GetFriendRequest(requestId, acceptId);
            if (existingRequest != null)
            {
                return false; // Request already exists
            }

            var friendRequest = new Friend
            {
                RequestId = requestId,
                AcceptId = acceptId,
                InvitationTime = DateTime.Now,
                IsAccept = null
            };
            await _friendRepository.AddFriendRequest(friendRequest);
            return true;
        }

        public async Task AcceptFriendRequest(string requestId, string acceptId) =>
            await _friendRepository.UpdateFriendRequestStatus(requestId, acceptId, true);

        public async Task  DeclineFriendRequest(string requestId, string acceptId) =>
            await _friendRepository.UpdateFriendRequestStatus(requestId, acceptId, false);

        public async Task<List<Friend>> GetPendingInvitations(string gamerId)
        {
            // Retrieve the gamer by ID
            var gamer = await _gamerRepository.GetGamerById(gamerId);

            // Check if the gamer is null and throw an exception if so
            if (gamer == null)
            {
                // Log and throw an exception if gamer is null
                Debug.WriteLine($"Gamer with ID {gamerId} not found.");
                throw new ArgumentNullException(nameof(gamer), $"Gamer with ID {gamerId} not found!");
            }

            // Log the found gamer ID
            Debug.WriteLine($"Gamer found: {gamer.GamerId}");

            // If the gamer exists, proceed to fetch invitations
            return _friendRepository.GetFriendInvitationsForGamer(gamer);
        }


    }

}
