using FLauncher.Repositories;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FLauncher.ViewModel
{
    public class ProfileWindowViewModel : INotifyPropertyChanged
    {
        private readonly IFriendRepository _friendRepository;
        private readonly IGamerRepository _gamerRepository;

        private int _friendCount;
        private string _avatarLink;
        private string _name;

        public int FriendCount
        {
            get { return _friendCount; }
            set
            {
                if (_friendCount != value)
                {
                    _friendCount = value;
                    OnPropertyChanged(nameof(FriendCount)); // Notify UI that FriendCount has changed
                }
            }
        }

        public string AvatarLink
        {
            get { return _avatarLink; }
            set
            {
                if (_avatarLink != value)
                {
                    _avatarLink = value;
                    OnPropertyChanged(nameof(AvatarLink)); // Notify UI that AvatarLink has changed
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name)); // Notify UI that Name has changed
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ProfileWindowViewModel(IFriendRepository friendRepository, IGamerRepository gamerRepository)
        {
            _friendRepository = friendRepository;
            _gamerRepository = gamerRepository;
        }

        // Method to load avatar, name, and friend count
        public async Task LoadProfileData(string gamerId)
        {
            // Fetch the gamer data from the repository
            var gamer = await _gamerRepository.GetGamerById(gamerId);

            if (gamer != null)
            {
                // Set the properties with the data from the database
                Name = gamer.Name;
                AvatarLink = gamer.AvatarLink;

                // Fetch friend count
                var friends = await _friendRepository.GetFriendsForGamer(gamer);
                FriendCount = friends.Count;
            }
            else
            {
                // If the gamer is not found, set default values
                Name = "Unknown";
                AvatarLink = "default_avatar.png";
                FriendCount = 0;
            }
        }

        // Refresh friend count method (called after accepting an invitation)
        public async Task RefreshFriendCount(string gamerId)
        {
            // Fetch the updated friend count for the gamer
            var gamer = await _gamerRepository.GetGamerById(gamerId);

            if (gamer != null)
            {
                // Fetch the updated list of friends and set the friend count
                var friends = await _friendRepository.GetFriendsForGamer(gamer);
                FriendCount = friends.Count;
            }
        }
    }
}
