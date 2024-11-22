using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.ViewModel
{
    public class SearchWindowViewModel : INotifyPropertyChanged
    {
        public Gamer Gamer { get; }
        public GamePublisher GamePublisher { get; }

        public int UnreadNotificationCount => UnreadNotifications?.Count ?? 0;
        public ObservableCollection<Notification> UnreadNotifications { get; }

        public int FriendInvitationCount => FriendInvitations?.Count ?? 0;
        public ObservableCollection<Friend> FriendInvitations { get; }

        // Using ObservableCollection for dynamic data binding
        public ObservableCollection<Game> AllGames { get; }

        public ObservableCollection<Genre> Genres { get; }

        public string Name => Gamer?.Name ?? GamePublisher?.Name;
        public double Money => Gamer?.Money ?? GamePublisher?.Money ?? 0.0;

        // Constructor for Gamer Role
        public SearchWindowViewModel(Gamer gamer, IEnumerable<Notification> unreadNotifications, IEnumerable<Friend> friendInvitations, IEnumerable<Game> allgames, IEnumerable<Genre> genres)
        {
            Gamer = gamer;
            UnreadNotifications = new ObservableCollection<Notification>(unreadNotifications);
            FriendInvitations = new ObservableCollection<Friend>(friendInvitations);
            AllGames = new ObservableCollection<Game>(allgames);  // ObservableCollection to notify UI changes
            Genres = new ObservableCollection<Genre>(genres);
        }

        // Constructor for Publisher Role
        public SearchWindowViewModel(GamePublisher gamePublisher, IEnumerable<Game> allgames, IEnumerable<Genre> genres)
        {
            GamePublisher = gamePublisher;
            AllGames = new ObservableCollection<Game>(allgames);
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
