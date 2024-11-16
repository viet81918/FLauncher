using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.ViewModel
{
    public class GameDetailViewModel
    {
        public Game Game { get; }
        public Gamer Gamer { get; }
        public ObservableCollection<Genre> Genres { get; }
        public List<Review> Reviews { get; }
        public int UnreadNotificationCount => UnreadNotifications?.Count ?? 0;
        public List<Notification> UnreadNotifications { get; }
        public int FriendInvitationCount => FriendInvitations?.Count ?? 0;
        public List<Friend> FriendInvitations { get; }
        public double AverageRating => Reviews?.Any() == true ? Reviews.Average(r => r.Rating) : 0;
        public GamePublisher GamePublisher { get; }
        public List<Update> Updates { get; }
        public string Name => Gamer?.Name ?? GamePublisher?.Name;
        public double Money => Gamer?.Money ?? GamePublisher?.Money ?? 0.0;
        public List<Gamer> Friendwiththesamegame {  get; }
        public GameDetailViewModel(Game game, Gamer gamer, IEnumerable<Genre> genres, List<Review> reviews, List<Notification> unreadNotifications, List<Friend> friendInvitations, GamePublisher gamePublisher, List<Update> updates, List<Gamer> friendwiththesamegame)

  
        {
            Game = game;
            Gamer = gamer;
            Genres = new ObservableCollection<Genre>(genres);
            Reviews = reviews;
            UnreadNotifications = unreadNotifications;
            FriendInvitations = friendInvitations;
            GamePublisher = gamePublisher;
            Updates = updates;
            Friendwiththesamegame = friendwiththesamegame;
        }

        public GameDetailViewModel(Game game, IEnumerable<Genre> genres, List<Review> reviews, GamePublisher gamePublisher, List<Update> updates)
        {
            Game = game;
            Genres = new ObservableCollection<Genre>(genres);
            Reviews = reviews;
            GamePublisher = gamePublisher;
            Updates = updates;
        }
    }


}
