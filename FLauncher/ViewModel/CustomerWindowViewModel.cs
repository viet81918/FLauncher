using FLauncher.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FLauncher.ViewModel
{
    public class CustomerWindowViewModel : INotifyPropertyChanged
    {
        public Gamer Gamer { get; }
        public GamePublisher GamePublisher { get; }

        public int UnreadNotificationCount => UnreadNotifications?.Count ?? 0;
        public ObservableCollection<Notification> UnreadNotifications { get; }

        public int FriendInvitationCount => FriendInvitations?.Count ?? 0;
        public ObservableCollection<Friend> FriendInvitations { get; }

        // Using ObservableCollection for dynamic data binding
        public ObservableCollection<Game> TrendingGames { get; }

        public ObservableCollection<Genre> Genres { get; }

        public string Name => Gamer?.Name ?? GamePublisher?.Name;
        public double Money => Gamer?.Money ?? GamePublisher?.Money ?? 0.0;

        // Constructor for Gamer Role
        public CustomerWindowViewModel(Gamer gamer, IEnumerable<Notification> unreadNotifications, IEnumerable<Friend> friendInvitations, IEnumerable<Game> trendingGames, IEnumerable<Genre> genres)
        {
            Gamer = gamer;
            UnreadNotifications = new ObservableCollection<Notification>(unreadNotifications);
            FriendInvitations = new ObservableCollection<Friend>(friendInvitations);
            TrendingGames = new ObservableCollection<Game>(trendingGames);  // ObservableCollection to notify UI changes
            Genres = new ObservableCollection<Genre>(genres);
        }

        // Constructor for Publisher Role
        public CustomerWindowViewModel(GamePublisher gamePublisher, IEnumerable<Game> trendingGames, IEnumerable<Genre> genres)
        {
            GamePublisher = gamePublisher;
            TrendingGames = new ObservableCollection<Game>(trendingGames);
            Genres = new ObservableCollection<Genre>(genres);
        }

        // Property changed event to notify UI
        public event PropertyChangedEventHandler PropertyChanged;

        // Helper method to raise the PropertyChanged event
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}