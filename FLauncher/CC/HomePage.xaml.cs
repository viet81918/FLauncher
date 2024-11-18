using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.ViewModel;
using System.Windows.Controls;

namespace FLauncher.CC
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private Gamer _gamer;
        private readonly GamerRepository _gamerRepo;
        private readonly NotiRepository _notiRepo;
        private readonly FriendRepository _friendRepo;
        private readonly GameRepository _gameRepo;
        public HomePage(User user)
        {
            InitializeComponent();
            _gamerRepo = new GamerRepository();
            _notiRepo = new NotiRepository();
            _friendRepo = new FriendRepository();
            _gameRepo = new GameRepository(); // Game repository to fetch games

            // Fetch gamer details
            _gamer = _gamerRepo.GetGamerByUser(user);

            // Fetch unread notifications, friend invitations, and games
            var unreadNotifications = _notiRepo.GetUnreadNotiforGamer(_gamer);
            var friendInvitations = _friendRepo.GetFriendInvitationsforGamer(_gamer);

            // Fetch all games and sort by NumberOfBuyers in descending order to get the top 6
            var allGames = _gameRepo.GetGames();
            var topGames = allGames.OrderByDescending(g => g.NumberOfBuyers).Take(9).ToList();

            // Set DataContext to the ViewModel
            DataContext = new CustomerWindowViewModel(_gamer, unreadNotifications, friendInvitations, topGames);

        }
    }
}
