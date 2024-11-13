using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.Services;
using FLauncher.ViewModel;
using FLauncher.Views;
using MongoDB.Driver;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace FLauncher
{
    public partial class CustomerWindow : Window
    {
        private Gamer _gamer;
        private GamePublisher _gamePublisher;
        private readonly PublisherRepository _publisherRepo;
        private readonly GamerRepository _gamerRepo;
        private readonly NotiRepository _notiRepo;
        private readonly FriendRepository _friendRepo;
        private readonly GameRepository _gameRepo;
        private readonly GenreRepository _genreRepo;
        private readonly FriendService _friendService;

        public CustomerWindow(User user)
        {
            InitializeComponent();
            //get userID == 2
            if (user.Role == 2)
            {
                settingsIconListBoxItem.Visibility = Visibility.Visible;
            }
            else
            {
                settingsIconListBoxItem.Visibility = Visibility.Collapsed;
            }
            //end 
            _gamerRepo = new GamerRepository();
            _notiRepo = new NotiRepository();
            _friendRepo = new FriendRepository();
            _gameRepo = new GameRepository(); // Game repository to fetch games
            _genreRepo = new GenreRepository();

            _publisherRepo = new PublisherRepository();
            // Fetch gamer details


            // Fetch unread notifications, friend invitations, and games
            //var unreadNotifications = _notiRepo.GetUnreadNotiforGamer(_gamer);
            //var friendInvitations = _friendRepo.GetFriendInvitationsforGamer(_gamer);

            // Fetch all games and sort by NumberOfBuyers in descending order to get the top 9
            var allGames = _gameRepo.GetGames();
            var topGames = allGames.OrderByDescending(g => g.NumberOfBuyers).Take(9).ToList();
            var genres = _genreRepo.GetGenres();
            if (user.Role == 3)
            {
                _gamer = _gamerRepo.GetGamerByUser(user);
                var unreadNotifications = _notiRepo.GetUnreadNotiforGamer(_gamer);
                var friendInvitations = _friendRepo.GetFriendInvitationsForGamer(_gamer);
                DataContext = new CustomerWindowViewModel(_gamer, unreadNotifications, friendInvitations, topGames, genres);

            }
            else if (user.Role == 2)
            {
                _gamePublisher = _publisherRepo.GetPublisherByUser(user);

                DataContext = new CustomerWindowViewModel(_gamePublisher, topGames, genres);
            }
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

        private void TrendingCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Get the clicked game from the data context
            var clickedGame = ((FrameworkElement)sender).DataContext as Game;

            if (clickedGame != null)
            {
                // Get the current gamer from the DataContext
                var currentGamer = _gamer;
                var currentPublisher = _gamePublisher;
                // Navigate to the GameDetail page and pass the selected game and gamer
                var gameDetailPage = new GameDetail(clickedGame, currentGamer, currentPublisher);
                gameDetailPage.Show();
            }
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
        private void messageButton_Click(Object sender, MouseButtonEventArgs e)
        {
            MessageWindow mess = new MessageWindow();
            var currentGamer = _gamer;
            var messDetail = new MessageWindow(currentGamer);
            mess.Show();
            this.Hide();
            this.Close();
        }
        private void logoutButton_Click(object sender, MouseButtonEventArgs e)
        {
            var result = MessageBox.Show("Bạn muốn đăng xuất?", "Xác nhận đăng xuất", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DeleteLoginInfoFile();
                this.Hide();
                Login login = new Login();
                login.Show();
                
                this.Close();
            }
        }
        private void DeleteLoginInfoFile()
        {
            string appDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FLauncher");
            string jsonFilePath = System.IO.Path.Combine(appDataPath, "loginInfo.json");

            if (File.Exists(jsonFilePath))
            {
                File.Delete(jsonFilePath);
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