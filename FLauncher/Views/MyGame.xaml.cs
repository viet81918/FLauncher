using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.Services;
using FLauncher.ViewModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FLauncher.Views
{
    /// <summary>
    /// Interaction logic for MyGame.xaml
    /// </summary>
    public partial class MyGame : Window
    {
        private User _user;
        private Gamer _gamer;
        private GamePublisher _gamePublisher;
        private readonly IPublisherRepository _publisherRepo;
        private readonly GamerRepository _gamerRepo;
        private readonly INotiRepository _notiRepo;
        private readonly FriendRepository _friendRepo;
        private readonly IGameRepository _gameRepo;
        private readonly IGenresRepository _genreRepo;
        private FriendService _friendService;
        private Game _game;

        private readonly IReviewRepository _reviewRepo;

        private readonly IUserRepository _userRepo;


        public MyGame(Game game, User user)
        {
            InitializeComponent();
            _user = user;
            _gamerRepo = new GamerRepository();
            _notiRepo = new NotiRepository();
            _friendRepo = new FriendRepository();
            _gameRepo = new GameRepository();
            _genreRepo = new GenreRepository();
            _publisherRepo = new PublisherRepository();
            _notiRepo = new NotiRepository();
            _friendRepo = new FriendRepository();


            _userRepo = new UserRepository();


            _reviewRepo = new ReviewRepository();

            InitializeData(game, user);
        }

        private async void InitializeData(Game game, User user)
        {
            if (user.Role == 3)
            {
                _gamer = _gamerRepo.GetGamerByUser(_user);
            }
            if (user.Role == 2)
            {
                _gamePublisher = _publisherRepo.GetPublisherByUser(_user);
            }
            // Fetch top games and genres asynchronously

            var genres = await _genreRepo.GetGenres();    // Assuming GetGenres() is async

            if (user.Role == 3) // Role 3 - Gamer
            {

                var games = await _gameRepo.GetGamesByGamer(_gamer);

                var unreadNotifications = await _notiRepo.GetUnreadNotiforGamer(_gamer);
                var friendInvitations = await _friendRepo.GetFriendInvitationsForGamer(_gamer);
                DataContext = new MyGameViewModel(_gamer, unreadNotifications, friendInvitations, games);
            }
            else if (user.Role == 2) // Role 2 - Publisher
            {
                _gamePublisher = _publisherRepo.GetPublisherByUser(user); // Assuming async

            }
            //var genres = await _genreRepo.GetGenresFromGame(game); // Get genres from your repository
            var reviews = await _reviewRepo.GetReviewsByGame(game); // Get reviews from your repository
            var publisher = await _publisherRepo.GetPublisherByGame(game);
            var updates = await _publisherRepo.getUpdatesForGame(game);
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
        private void messageButton_Click(Object sender, MouseButtonEventArgs e)

        {
            var currentGamer = _gamer;
            MessageWindow messWindow = new MessageWindow(currentGamer);
            messWindow.Show();

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
            _friendService = new FriendService(_friendRepo, _gamerRepo);

            ProfileWindow profileWindow = new ProfileWindow(_gamer, _friendService);
            profileWindow.Show();
            this.Hide();
            this.Close();

        }

        private void GamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Populate the right column with the details of the selected game.
        }

        private void FavoritesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Populate the right column with the details of the selected favorite game.
        }

        private void Home_Click(object sender, MouseButtonEventArgs e)
        {
            CustomerWindow cus = new CustomerWindow(_user);
            cus.Show();
            this.Hide();
            this.Close();
        }

    }
}
