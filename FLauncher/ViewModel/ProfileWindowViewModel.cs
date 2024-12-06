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
using System.Windows.Threading;

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
        public ObservableCollection<Achivement> UnlockAchivement { get; }
        public ObservableCollection<UnlockAchivement> Unlock { get; }
        public ObservableCollection<UnlockAchivementViewModel> UnlockAchivementViewModels { get; set; }
        public ObservableCollection<TrackingMyGameViewModel> TrackingMyGames { get; set; }

        // Constructor to initialize the ViewModel with repositories
        public ProfileWindowViewModel(Gamer gamer, IFriendRepository friendRepository,
            IGamerRepository gamerRepository, IEnumerable<Gamer> friends,
            IEnumerable<Achivement> UnlockAchivements,
            IEnumerable<UnlockAchivement> unlockAchivementsData,
            IEnumerable<(Game Game, double TotalHours, DateTime LastPlayed)> gameWithHours)
        {
            Gamer = gamer;
            IsGamer = true;

            _friendRepository = friendRepository;
            _gamerRepository = gamerRepository;
            ListFriend = new ObservableCollection<Gamer>(friends);

            // Assign the unlocked achievements to the UnlockAchievements ObservableCollection
            UnlockAchivement = new ObservableCollection<Achivement>(UnlockAchivements);

            // Initialize the UnlockAchievementViewModels collection
            UnlockAchivementViewModels = new ObservableCollection<UnlockAchivementViewModel>();

            // Populate the UnlockAchievementViewModels collection
            foreach (var unlock in unlockAchivementsData)
            {
                var achievement = UnlockAchivement.FirstOrDefault(a => a.AchivementId == unlock.AchievementId && a.GameId == unlock.GameId);
                if (achievement != null)
                {
                    UnlockAchivementViewModels.Add(new UnlockAchivementViewModel
                    {
                        Name = achievement.Name,
                        UnlockImageLink = achievement.UnlockImageLink,
                        DateUnlockString = unlock.DateUnlockString,
                        AchivmentId = unlock.AchievementId,
                        GameId = unlock.GameId,
                        GamerId = unlock.GamerId
                    });
                }
            }
            // Populate TrackingMyGames
            TrackingMyGames = new ObservableCollection<TrackingMyGameViewModel>(
                gameWithHours.Select(g => new TrackingMyGameViewModel
                {
                    GameName = g.Game.Name, 
                    GameImage =g.Game.AvatarLink, 
                    TotalPlayingHours = g.TotalHours,       // Use TotalHours provided
                    LastPlayedDate = g.LastPlayed.ToString("dd/MM/yyyy") // Format LastPlayed
                }));

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
