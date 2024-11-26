using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.Services;
using FLauncher.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FLauncher.ViewModel
{
    public class ProfileWindowViewModel : INotifyPropertyChanged
    {
        public User user;
        private readonly IFriendRepository _friendRepository;
        private readonly IGamerRepository _gamerRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly IUserRepository _userRepository;
        private  FriendService _friendService;
        public Gamer Gamer { get; }
        public GamePublisher GamePublisher { get; }

        // Properties for dynamic data binding
        public ObservableCollection<Gamer> ListFriend { get; }
        public int FriendCount { get; private set; }
        public string AvatarLink { get; private set; }
        public string Name { get; private set; }
        public bool IsGamer { get; set; }
        public bool IsFriend { get; set; }

        private bool _hasPendingInvitation;
        public bool HasPendingInvitation
        {
            get => _hasPendingInvitation;
            set
            {
                _hasPendingInvitation = value;
                OnPropertyChanged(nameof(HasPendingInvitation));
            }
        }

        private bool _isCurrentUser;
        public bool IsCurrentUser
        {
            get => _isCurrentUser;
            set
            {
                _isCurrentUser = value;
                OnPropertyChanged(nameof(IsCurrentUser));
            }
        }


        // Constructor to initialize the ViewModel with repositories
        public ProfileWindowViewModel(Gamer gamer, IFriendRepository friendRepository, IGamerRepository gamerRepository, IEnumerable<Gamer> friends)
        {
            Gamer = gamer;
            IsGamer = true;
             
            _friendRepository = friendRepository;
            _gamerRepository = gamerRepository;
            ListFriend = new ObservableCollection<Gamer>(friends);

            
        }
       

        public async Task LoadFriendStatusAsync(string viewerId)
        {
            IsFriend = await _friendRepository.IsFriend(viewerId, Gamer.GamerId);
            OnPropertyChanged(nameof(IsFriend)); // Notify UI of property change
        }


        // Method to load friends from the repository


        // Method to load profile data (name, avatar, and friend count)
        public async Task LoadProfileData(User user)
        {
            if (user.Role == 3) // Role 3 - Gamer
            {
                // Retrieve Gamer info
                var gamer = _gamerRepository.GetGamerByUser(user);
                if (gamer != null)
                {
                    Name = gamer.Name;
                    AvatarLink = gamer.AvatarLink;

                    // Load and set friend count
                    var friends = await _friendRepository.GetFriendsForGamer(gamer);
                    FriendCount = friends.Count;
                }
                else
                {
                    Name = "Unknown Gamer";
                    AvatarLink = "default_avatar.png";
                    FriendCount = 0;
                }
            }
            else if (user.Role == 2) // Role 2 - Game Publisher
            {
                // Retrieve Game Publisher info
                var gamePublisher = _publisherRepository.GetPublisherByUser(user);
                if (gamePublisher != null)
                {
                    Name = gamePublisher.Name;
                    AvatarLink = gamePublisher.AvatarLink;

                    // Publishers don't have friends; set FriendCount to 0 or any relevant value
                    FriendCount = 0;
                }
                else
                {
                    Name = "Unknown Publisher";
                    AvatarLink = "default_avatar.png";
                    FriendCount = 0;
                }
            }
            else
            {
                // Handle other roles or invalid role cases
                Name = "Unknown User";
                AvatarLink = "default_avatar.png";
                FriendCount = 0;
            }

            // Notify the UI of property changes
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(AvatarLink));
            OnPropertyChanged(nameof(FriendCount));
        }


        // Method to refresh the friend count after changes

        public void UpdateFriendsList(IEnumerable<Gamer> updatedFriends)
        {
            // Clear the existing list
            ListFriend.Clear();

            // Add the updated list of friends
            foreach (var friend in updatedFriends)
            {
                ListFriend.Add(friend);
            }
        }


        public async Task RefreshFriendCount(string gamerId)
        {
            var gamer = _gamerRepository.GetGamerById(gamerId);
            if (gamer != null)
            {
                var friends = await _friendRepository.GetFriendsForGamer(gamer);
                FriendCount = friends.Count;

                OnPropertyChanged(nameof(FriendCount)); // Notify the UI of the updated friend count
            }
        }

        // Property changed event to notify the UI of changes
        public event PropertyChangedEventHandler PropertyChanged;

        // Helper method to raise the PropertyChanged event
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



     


       
    }
}
