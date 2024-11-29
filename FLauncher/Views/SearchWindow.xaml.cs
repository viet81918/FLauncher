using Azure.Messaging;
using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.Services;
using FLauncher.ViewModel;
using Microsoft.IdentityModel.Tokens;
using SharpCompress;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ZstdSharp.Unsafe;


namespace FLauncher.Views
{    
    public partial class SearchWindow : Window
    {
        private Gamer _gamer;
        private User _user;
        private IEnumerable<Game> allGames;
        private IEnumerable<Genre> genres;
        private IEnumerable<Notification> unreadNotifications;
        private IEnumerable<Friend> friendInvitations;
        private GamePublisher _gamePublisher;
        private readonly IPublisherRepository _publisherRepo;
        private readonly GamerRepository _gamerRepo;
        private readonly UserRepository _userRepo;
        private readonly FriendRepository _friendRepo;
        private readonly FriendService _friendService;
        private readonly IGameRepository _gameRepo;
        private readonly IGenresRepository _genreRepo;
        private readonly INotiRepository _notiRepo;

        private List<string> selectedGenre;
        private string selectedPub;
        public SearchWindow(User user, string inputSearch,List<string> GenreSearch, string PublisherSearch)
        {
            InitializeComponent();
            if (user.Role == 2) // Giả sử 1 là Publisher
            {
                MessageButon.Visibility = Visibility.Collapsed; // Ẩn
                profileButton.Visibility = Visibility.Collapsed;
            }
            else if (user.Role == 3) // Giả sử 2 là Gamer
            {
                MessageButon.Visibility = Visibility.Visible; // Hiện
                profileButton.Visibility = Visibility.Visible;
            }
            _userRepo = new UserRepository();
            _friendRepo = new FriendRepository();
            _publisherRepo = new PublisherRepository();
            _gameRepo = new GameRepository();
            _genreRepo = new GenreRepository();
            _notiRepo = new NotiRepository();
            _gamerRepo = new GamerRepository();
            _user = user;
            allGames = new List<Game>();
            genres = new List<Genre>();
            unreadNotifications = new List<Notification>();
            friendInvitations = new List<Friend>();
            InitializeData(user, inputSearch, GenreSearch, PublisherSearch);

            // Tìm đối tượng filterItems được khai báo trong XAML : Genre
            var filterControl = FindName("filterControl") as FLauncher.CC.filterItems;
            if (filterControl != null)
            {
                // Lắng nghe sự kiện GenreSelected
                filterControl.selectedGenre += OnGenreSelected;
            }
            else { MessageBox.Show("ko thay genre filter search window"); }

            // Tìm đối tượng filterItems được khai báo trong XAML : Publiser
            var filterPubControl = FindName("filterPublisherControl") as FLauncher.CC.filterItemsPub;
            if (filterPubControl != null)
            {
                filterPubControl.selectedPub += OnPubSelected;
            }
            else { MessageBox.Show("ko thay publisher filter search window"); }
        }
        private async void InitializeData(User user, string inputSearch, List<string> GenreSearch, string PublisherSearch)
        {        
            // Fetch top games and genres asynchronously
            
            if (!inputSearch.IsNullOrEmpty() || !GenreSearch.IsNullOrEmpty() || !PublisherSearch.IsNullOrEmpty()) // 1 trong 3 thang co value
            {               
                allGames = await _gameRepo.GetGameByInformation(inputSearch, GenreSearch, PublisherSearch);               
            }
            else // 3 thằng đều rỗng
            {             
                allGames = await _gameRepo.GetAllGame();
            }

            
            
            genres = await _genreRepo.GetGenres();    // Assuming GetGenres() is async

            if (user.Role == 3) // Role 3 - Gamer
            {
                _gamer = _gamerRepo.GetGamerByUser(user); // Assuming GetGamerByUserAsync() is async
                 unreadNotifications = await _notiRepo.GetUnreadNotiforGamer(_gamer); // Assuming async
                 friendInvitations = await _friendRepo.GetFriendInvitationsForGamer(_gamer); // Assuming async

                DataContext = new SearchWindowViewModel(_gamer, unreadNotifications, friendInvitations, allGames, genres);
            }
            else if (user.Role == 2) // Role 2 - Publisher
            {
                _gamePublisher = _publisherRepo.GetPublisherByUser(user); // Assuming async
                DataContext = new SearchWindowViewModel(_gamePublisher, allGames, genres);
            }
        }
        // Xử lý khi một danh sach genre va pub được chọn
        private void OnGenreSelected(List<string> genre)
        {
            // Xử lý logic với giá trị genre được chọn
            MessageBox.Show($"Genre được chọn search window: {genre}");
            selectedGenre = genre;
            string selectedGenresText = string.Join(", ", selectedGenre);
            MessageBox.Show($"Các genre được chọn search window: {selectedGenresText}");
        }
        private void OnPubSelected(string publisher)
        {
            // Xử lý logic với giá trị publisher được chọn
            MessageBox.Show($"Publisher được chọn search window: {publisher}");
            selectedPub = publisher;
            string selectedPubsText = string.Join(", ", selectedPub);
            MessageBox.Show($"Các publisher được chọn search window: {selectedPubsText}");
        }
        private async void searchGame_button(object sender, RoutedEventArgs e)
        {
            string Search_input = SearchTextBox.Text.Trim().ToLower();
            if (Search_input == "search name game")
            {
                Search_input = string.Empty;
            }
            if (!string.IsNullOrEmpty(Search_input) || !selectedGenre.IsNullOrEmpty() || !selectedPub.IsNullOrEmpty())
            {
                //allGames = await _gameRepo.GetGameByName(Search_input);
                allGames = await _gameRepo.GetGameByInformation(Search_input, selectedGenre, selectedPub);
                if (_user.Role == 3) DataContext = new SearchWindowViewModel(_gamer, unreadNotifications, friendInvitations, allGames, genres);
                if(_user.Role == 2) DataContext = new SearchWindowViewModel(_gamePublisher, allGames, genres);
                SearchTextBox.Clear();
            }
            else
            {
                allGames = await _gameRepo.GetAllGame();
                if (_user.Role == 3) DataContext = new SearchWindowViewModel(_gamer, unreadNotifications, friendInvitations, allGames, genres);
                if (_user.Role == 2) DataContext = new SearchWindowViewModel(_gamePublisher, allGames, genres);
            }
        }

        private void TrendingCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Get the clicked game from the data context
            var clickedGame = ((FrameworkElement)sender).DataContext as Game;

            if (clickedGame != null)
            {
                // Get the current gamer from the DataContext
                
                // Navigate to the GameDetail page and pass the selected game and gamer
                var gameDetailPage = new GameDetail(clickedGame,_user );
                gameDetailPage.Show();
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
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            //Close the App
            Close();
        }
        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            //First detect if windows is in normal state or maximized
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }
        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Search name game")
            {
                SearchTextBox.Text = string.Empty;
                SearchTextBox.Foreground = (System.Windows.Media.Brush)Application.Current.Resources["SecondaryBrush"];
            }
        }
        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Search name game";
            }
        }
        private void messageButton_Click(Object sender, MouseButtonEventArgs e)
        {
            var currentGamer = _gamer;
            var messWindow = new MessageWindow(currentGamer);
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
            ProfileWindow profileWindow = new ProfileWindow(_user, _friendService);
            profileWindow.Show();
            this.Hide();
            this.Close();
            // Optionally, close the current window (MainWindow)
            // this.Close();
        }
        private void Home_Click(object sender, MouseButtonEventArgs e)
        {
            CustomerWindow cus = new CustomerWindow(_user);
            cus.Show();
            this.Hide();
            this.Close();
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                searchGame_button(sender, e);
            }
        }
        private void MyGame_Click(object sender, RoutedEventArgs e)
        {

            MyGame myGameWindow = new MyGame(_user);
            myGameWindow.Show();
            this.Hide();
            this.Close();
        }
    }
}
