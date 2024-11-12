using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.ViewModel
{
    public class GameDetailViewModel
    {
        public Game Game { get; }
        public Gamer Gamer { get; }
        public List<Genre> Genres { get; }
        public List<Review> Reviews { get; }
        public int UnreadNotificationCount => UnreadNotifications?.Count ?? 0;
        public List<Notification> UnreadNotifications { get; }
        public int FriendInvitationCount => FriendInvitations?.Count ?? 0;
        public List<Friend> FriendInvitations { get; }
        public double AverageRating => Reviews?.Any() == true ? Reviews.Average(r => r.Rating) : 0;
        public GamePublisher GamePublisher { get; }
        public List<Update> Updates { get; }
        // Common properties for both Gamer and GamePublisher
        public string Name => Gamer?.Name ?? GamePublisher?.Name;
        public double Money => Gamer?.Money ?? GamePublisher?.Money ?? 0.0;
        public GameDetailViewModel(Game game, Gamer gamer, List<Genre> genres, List<Review> reviews, List<Notification> unreadNotifications, List<Friend> friendInvitations, GamePublisher gamePublisher, List<Update> updates)
        {
            Game = game;
            Gamer = gamer;
            Genres = genres;
            Reviews = reviews;
            UnreadNotifications = unreadNotifications;
            FriendInvitations = friendInvitations;
            GamePublisher = gamePublisher;
            Updates = updates;
        }

        public GameDetailViewModel(Game game, List<Genre> genres, List<Review> reviews, GamePublisher gamePublisher, List<Update> updates)
        {
            Game = game;
            Genres = genres;
            Reviews = reviews;
            GamePublisher = gamePublisher;
            Updates = updates;
        }
    }


}
