using FLauncher.Model;
using FLauncher.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FLauncher.ViewModel
{
    public class ProfileWindowViewModel : INotifyPropertyChanged
    {
        private readonly IFriendRepository _friendRepository;
        private readonly IGamerRepository _gamerRepository;

        // Properties for dynamic data binding
        public ObservableCollection<Gamer> ListFriend { get; }
        public int FriendCount { get; private set; }
        public string AvatarLink { get; private set; }
        public string Name { get; private set; }

        // Constructor to initialize the ViewModel with repositories
        public ProfileWindowViewModel(IFriendRepository friendRepository, IGamerRepository gamerRepository, IEnumerable<Gamer> friends)
        {
            _friendRepository = friendRepository;
            _gamerRepository = gamerRepository;

            
            ListFriend = new ObservableCollection<Gamer>(friends);
        }

        // Method to load friends from the repository
       

        // Method to load profile data (name, avatar, and friend count)
        public async Task LoadProfileData(string gamerId)
        {
            var gamer = _gamerRepository.GetGamerById(gamerId);
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
                Name = "Unknown";
                AvatarLink = "default_avatar.png";
                FriendCount = 0;
            }

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(AvatarLink));
            OnPropertyChanged(nameof(FriendCount));
        }

        // Method to refresh the friend count after changes

      
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
