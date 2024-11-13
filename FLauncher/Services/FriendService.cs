using FLauncher.DAO;
using FLauncher.Model;
using FLauncher.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MongoDB.Bson;

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

        public bool AreGamersAlreadyFriends(string gamerId1, string gamerId2)
        {
            var existingFriendship = _friendRepository.GetFriendship(gamerId1, gamerId2).Result;  // Blocking call

            return existingFriendship != null && existingFriendship.IsAccept == true;
        }


        public bool SendFriendRequest(string requestId, string acceptId)
        {
            // Synchronously fetch the existing friend request
            var existingRequest = _friendRepository.GetFriendRequest(requestId, acceptId).Result; // Blocking call

            if (existingRequest != null)
            {
                return false; // Request already exists
            }

            // Create the friend request
            var friendRequest = new Friend
            {
                Id = ObjectId.GenerateNewId().ToString(), // Sinh một ObjectId mới
                RequestId = requestId,
                AcceptId = acceptId,
                InvitationTime = DateTime.Now,
                IsAccept = null
            };

            // Add the new friend request synchronously
            _friendRepository.AddFriendRequest(friendRequest).Wait(); // Blocking call

            return true;
        }


        public async Task AcceptFriendRequest(string requestId, string acceptId) =>
            await _friendRepository.UpdateFriendRequestStatus(requestId, acceptId, true);

        public async Task  DeclineFriendRequest(string requestId, string acceptId) =>
            await _friendRepository.UpdateFriendRequestStatus(requestId, acceptId, false);

        public List<Friend> GetPendingInvitations(string gamerId)
        {
            // Retrieve the gamer by ID synchronously
            var gamer = _gamerRepository.GetGamerById(gamerId);

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
            return _friendRepository.GetFriendInvitationsForGamer(gamer);  // Now using synchronous method
        }




    }

}
