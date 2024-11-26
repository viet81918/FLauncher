using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.ViewModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace FLauncher.Views
{

    public partial class GameDetail : Window


    {
        private Game _game = null;
        private Gamer _gamer = null;
        private Model.User _user;
        private GamePublisher _gamePublisher = null;
        private readonly INotiRepository _notiRepo;
        private readonly IFriendRepository _friendRepo;
        private readonly IGameRepository _gameRepo;
        private readonly IGenresRepository _genreRepo;
        private readonly IReviewRepository _reviewRepo;
        private readonly IPublisherRepository _publisherRepo;


        private readonly IGamerRepository _gamerRepo;
        private readonly IUserRepository _userRepo;
        public GameDetail(Game game, Model.User user)
        {
            InitializeComponent();
            if (user.Role == 2) // Giả sử 1 là Publisher
            {
                MessageButon.Visibility = Visibility.Collapsed; // Ẩn
            }
            else if (user.Role == 3) // Giả sử 2 là Gamer
            {
                MessageButon.Visibility = Visibility.Visible; // Hiện
            }
            _notiRepo = new NotiRepository();
            _friendRepo = new FriendRepository();

            _gameRepo = new GameRepository();

            _userRepo = new UserRepository();
            _user = user;
            _genreRepo = new GenreRepository();
            _reviewRepo = new ReviewRepository();
            _publisherRepo = new PublisherRepository();
            _gamerRepo = new GamerRepository();
            InitializeData(game, user);


        }
        private async void InitializeData(Game game, Model.User user)
        {
            _game = game;
            if (user.Role == 3)
            {
                _gamer = _gamerRepo.GetGamerByUser(user);
            }
            else
            {
                _gamePublisher = _publisherRepo.GetPublisherByUser(user);
            }



            var genres = await _genreRepo.GetGenresFromGame(game); // Get genres from your repository
            var reviews = await _reviewRepo.GetReviewsByGame(game); // Get reviews from your repository
            var publisher = await _publisherRepo.GetPublisherByGame(game);
            var updates = await _publisherRepo.getUpdatesForGame(game);

            // Set the DataContext to your ViewModel            
            if (_gamer != null)
            {
                var friendwithsamegame = await _friendRepo.GetFriendWithTheSameGame(game, _gamer);
                var unreadNotifications = await _notiRepo.GetUnreadNotiforGamer(_gamer);
                var friendInvitations = await _friendRepo.GetFriendInvitationsForGamer(_gamer);
                var Achivements = await _gameRepo.GetAchivesFromGame(_game);
                var Unlock = await _gameRepo.GetUnlockAchivementsFromGame(Achivements, _gamer);
                var UnlockAchivements = await _gameRepo.GetAchivementsFromUnlocks(Unlock);
              
                var LockAchivements = await _gameRepo.GetLockAchivement(Achivements, _gamer);
                var reviewers = await _gamerRepo.GetGamersFromGame(game);
                var isBuy = await _gameRepo.IsBuyGame(game, _gamer);
                var isUpdate = await _gamerRepo.IsUpdate(game, _gamer);
                var isDownLoad = await _gameRepo.isDownload(game, _gamer);
                DataContext = new GameDetailViewModel(game, _gamer, genres, reviews, unreadNotifications, friendInvitations, publisher, updates, friendwithsamegame, UnlockAchivements, Achivements, LockAchivements, Unlock, reviewers, isBuy, isDownLoad, isUpdate);
            }
            else if (_gamePublisher != null)
            {
                var isPublish = await _gameRepo.IsPublishGame(game, _gamePublisher);
                var gamers = await _gamerRepo.GetGamersFromGame(game);
                var Achivements = await _gameRepo.GetAchivesFromGame(_game);

                DataContext = new GameDetailViewModel(game,genres ,reviews, publisher, updates, isPublish, Achivements, gamers);
            }
        }

        private void Polygon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //To move the window on mouse down
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        private void Download_Click(object sender, RoutedEventArgs e)
        {
            string location = Microsoft.VisualBasic.Interaction.InputBox("Enter the location your want to download the game :", "Download");

            if (string.IsNullOrWhiteSpace(location))
            {
                System.Windows.MessageBox.Show("Please enter a valid Location.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _gameRepo.Download_game(_game, location, _gamer);
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {

            _gameRepo.Play_Game(_game, _gamer);
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            // Bước 1: Tạo OpenFileDialog để chọn file
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true) // Đúng kiểu bool
            {
                // Lấy đường dẫn file đã chọn
                string selectedFilePath = openFileDialog.FileName;

                // Bước 2: Hiển thị InputBox để người dùng nhập message
                string message = Microsoft.VisualBasic.Interaction.InputBox(
                    "Please enter your message:",
                    "Input Message",
                    "Default message here...",
                    -1, -1
                );

                if (string.IsNullOrWhiteSpace(message))
                {
                    System.Windows.MessageBox.Show("Message is empty or cancelled!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Gọi hàm cập nhật game
                _gameRepo.Upload_game(_gamePublisher, _game, selectedFilePath, message);
            }
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
        private void Uninstall_Click(object sender, RoutedEventArgs e)
        {
            _gameRepo.Reinstall(_game, _gamer);
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Search name game")
            {
                SearchTextBox.Text = string.Empty;
                SearchTextBox.Foreground = (System.Windows.Media.Brush)System.Windows.Application.Current.Resources["SecondaryBrush"];
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Search name game";
            }
        }
        private void ReinstallGame_click(object sender, RoutedEventArgs e)
        {
            _gameRepo.Reinstall(_game, _gamer);
        }
        private void Tracking_Click(object sender, RoutedEventArgs e)
        {
            var gameDetailPage = new TrackingTimePlayed(_gamer, _game);
            gameDetailPage.Show();
        }
        


          private void TrackingPlayers_Click(object sender, RoutedEventArgs e)
        {
            var gameDetailPage = new TrackingNumberPlayer( _game);
            gameDetailPage.Show();
        }
        private void Achivement_Click(object sender, RoutedEventArgs e)
        {
            var gameDetailPage = new AchivementManagement(_user,_game);
            gameDetailPage.Show();
        }
        private void Home_Click(object sender, MouseButtonEventArgs e)
        {
            CustomerWindow cus = new CustomerWindow(_user);
            cus.Show();
            this.Hide();
            this.Close();
        }
        private void MyGame_Click(object sender, RoutedEventArgs e)
        {

            MyGame myGameWindow = new MyGame(_user);
            myGameWindow.Show();
            this.Hide();
            this.Close();
        }
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchGame_button(sender, e);
            }
        }
        private void searchGame_button(object sender, RoutedEventArgs e)
        {
            var CurrentWin = _user;
            string Search_input = SearchTextBox.Text.Trim().ToLower();
            if (Search_input == "search name game")
            {
                Search_input = string.Empty;
            }
            SearchWindow search = new SearchWindow(CurrentWin, Search_input, null, null);
            search.Show();
            this.Hide();
            this.Close();
        }
        private void searchButton_Click(object sendedr, MouseButtonEventArgs e)
        {
            var CurrentUser = _user;
            SearchWindow serchwindow = new SearchWindow(CurrentUser, null, null, null);
            serchwindow.Show();
            this.Hide();
            this.Close();
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
    }
}