using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.Services;
using FLauncher.ViewModel;
using FLauncher.Views;
using System.Windows;
using System.Windows.Input;

namespace FLauncher
{
    public partial class CustomerWindow : Window
    {
        private Gamer _gamer;
        private readonly GamerRepository _gamerRepo;
        private readonly NotiRepository _notiRepo;
        private readonly FriendRepository _friendRepo;
        private readonly GameRepository _gameRepo;
        private readonly FriendService _friendService;

        public CustomerWindow(User user)
        {
            InitializeComponent();
            _gamerRepo = new GamerRepository();
            _notiRepo = new NotiRepository();
            _friendRepo = new FriendRepository();
            _gameRepo = new GameRepository(); // Game repository to fetch games

            // Instantiate FriendService with the friend repository
            _friendService = new FriendService(_friendRepo, _gamerRepo);

            // Fetch gamer details
            _gamer = _gamerRepo.GetGamerByUser(user);

            // Fetch unread notifications, friend invitations, and games
            var unreadNotifications = _notiRepo.GetUnreadNotiforGamer(_gamer);
            var friendInvitations = _friendRepo.GetFriendInvitationsForGamer(_gamer);

            // Fetch all games and sort by NumberOfBuyers in descending order to get the top 6
            var allGames = _gameRepo.GetGames();
            var topGames = allGames.OrderByDescending(g => g.NumberOfBuyers).Take(6).ToList();

            // Set DataContext to the ViewModel
            DataContext = new CustomerWindowViewModel(_gamer, unreadNotifications, friendInvitations, topGames);
        }
        private void Polygon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //To move the window on mouse down
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            //First detect if windows is in normal state or maximized
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            //Close the App
            Close();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Search the store")
            {
                SearchTextBox.Text = string.Empty;
                SearchTextBox.Foreground = (System.Windows.Media.Brush)Application.Current.Resources["SecondaryBrush"];
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Search the store";
            }
        }

        private void ProfileIcon_Click(object sender, MouseButtonEventArgs e)
        {
            // Create an instance of ProfileWindow and show it
            ProfileWindow profileWindow = new ProfileWindow(_gamer, _friendService);
            profileWindow.Show();

            // Optionally, close the current window (MainWindow)
            // this.Close();
        }

    }
}