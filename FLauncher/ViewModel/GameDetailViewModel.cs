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
        public List<Gamer> Friendwiththesamegame {  get; }
        public GameDetailViewModel(Game game, Gamer gamer, List<Genre> genres, List<Review> reviews, List<Notification> unreadNotifications, List<Friend> friendInvitations, GamePublisher gamePublisher, List<Update> updates, List<Gamer> friendwiththesamegame)
        {
            Game = game;
            Gamer = gamer;
            Genres = genres;
            Reviews = reviews;
            UnreadNotifications = unreadNotifications;
            FriendInvitations = friendInvitations;
            GamePublisher = gamePublisher;
            Updates = updates;
            Friendwiththesamegame = friendwiththesamegame;
        }
    }


}
