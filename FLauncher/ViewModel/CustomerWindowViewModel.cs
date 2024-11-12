using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.ViewModel
{
    public class CustomerWindowViewModel
    {
        public Gamer Gamer { get; }
        public GamePublisher GamePublisher { get; }
        public int UnreadNotificationCount => UnreadNotifications?.Count ?? 0;
        public List<Notification> UnreadNotifications { get; }

        public int FriendInvitationCount => FriendInvitations?.Count ?? 0;
        public List<Friend> FriendInvitations { get; }

        // Add the TrendingGames property
        public List<Game> TrendingGames { get; }

        public List<Genre> Genres {  get; }

        // Common properties for both Gamer and GamePublisher
        public string Name => Gamer?.Name ?? GamePublisher?.Name;
        public double Money => Gamer?.Money ?? GamePublisher?.Money ?? 0.0;
        public CustomerWindowViewModel(Gamer gamer, List<Notification> unreadNotifications, List<Friend> friendInvitations, List<Game> trendingGames, List<Genre> genres) 
        {
            Gamer = gamer;
            UnreadNotifications = unreadNotifications;
            FriendInvitations = friendInvitations;
            TrendingGames = trendingGames;
            Genres = genres;
        }
        
        public CustomerWindowViewModel(GamePublisher gamePublisher, List<Game> trendingGames, List<Genre> genres)
        {
            GamePublisher = gamePublisher;            
            TrendingGames = trendingGames;
            Genres = genres;
        }
    }




}
