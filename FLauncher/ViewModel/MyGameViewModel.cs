using FLauncher.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FLauncher.ViewModel
{
    public class MyGameViewModel : INotifyPropertyChanged
    {
        public Gamer Gamer { get; }

        public int UnreadNotificationCount => UnreadNotifications?.Count ?? 0;
        public ObservableCollection<Notification> UnreadNotifications { get; }

        public int FriendInvitationCount => FriendInvitations?.Count ?? 0;
        public ObservableCollection<Friend> FriendInvitations { get; }

        public string Name => Gamer?.Name;
        public double Money => Gamer?.Money ?? 0.0;
        public ObservableCollection<Game> MyGames { get; }
        public MyGameViewModel(Gamer gamer, IEnumerable<Notification> unreadNotifications, IEnumerable<Friend> friendInvitations, IEnumerable<Game> myGames)
        {

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
