using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.Services;
using FLauncher.Utilities;
using FLauncher.ViewModel;
using FLauncher.Views;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace FLauncher
{
    public partial class CustomerWindow : Window
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


        private List<string> selectedGenre;
        private string selectedPub;
        public CustomerWindow(User user)
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

            _user = user;
            _gamerRepo = new GamerRepository();
            _notiRepo = new NotiRepository();
            _friendRepo = new FriendRepository();
            _gameRepo = new GameRepository();
            _genreRepo = new GenreRepository();
            _publisherRepo = new PublisherRepository();
            _user = user;
            InitializeData(user);

            // Tìm đối tượng filterItems được khai báo trong XAML : Genre
            var filterControl = FindName("filterControl") as FLauncher.CC.filterItems;
            if (filterControl != null)
            {
                // Lắng nghe sự kiện GenreSelected
                filterControl.selectedGenre += OnGenreSelected;
            }
            else { MessageBox.Show("ko thay genre filter"); }

            // Tìm đối tượng filterItems được khai báo trong XAML : Publiser
            var filterPubControl = FindName("filterPublisherControl") as FLauncher.CC.filterItemsPub;
            if (filterPubControl != null)
            {
                filterPubControl.selectedPub += OnPubSelected;
            }
            else { MessageBox.Show("ko thay publisher filter"); }

        }
        // Xử lý khi một danh sach genre va pub được chọn
        private void OnGenreSelected(List<string> genre)
        {
            // Xử lý logic với giá trị genre được chọn
            MessageBox.Show($"Genre được chọn: {genre}");
            selectedGenre = genre;
            string selectedGenresText = string.Join(", ", selectedGenre);
            MessageBox.Show($"Các genre được chọn: {selectedGenresText}");
        }
        private void OnPubSelected(string publisher)
        {
            // Xử lý logic với giá trị publisher được chọn
            MessageBox.Show($"Publisher được chọn: {publisher}");
            selectedPub = publisher;
            string selectedPubsText = string.Join(", ", selectedPub);
            MessageBox.Show($"Các publisher được chọn: {selectedPubsText}");
        }

        private async void InitializeData(User user)
        {

            // Fetch top games and genres asynchronously
            var topGames = await _gameRepo.GetTopGames();  // Assuming GetTopGames() is async
            var genres = await _genreRepo.GetGenres();    // Assuming GetGenres() is async
            if (user.Role == 3) // Role 3 - Gamer
            {
                _gamer = _gamerRepo.GetGamerByUser(user); // Assuming GetGamerByUserAsync() is async
                var unreadNotifications = await _notiRepo.GetUnreadNotiforGamer(_gamer); // Assuming async
                var friendInvitations = await _friendRepo.GetFriendInvitationsForGamer(_gamer); // Assuming async
                var topPublishersData = await _publisherRepo.GetTopPublishersAsync();
                DataContext = new CustomerWindowViewModel(topPublishersData, _gamer, unreadNotifications, friendInvitations, topGames, genres);
            }
            else if (user.Role == 2) // Role 2 - Publisher
            {
                _gamePublisher = _publisherRepo.GetPublisherByUser(user); // Assuming async
                var topPublishersData = await _publisherRepo.GetTopPublishersAsync();
                DataContext = new CustomerWindowViewModel(topPublishersData, _gamePublisher, topGames, genres);
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
                var currentUser = _user;
                var currentPublisher = _gamePublisher;
                // Navigate to the GameDetail page and pass the selected game and gamer
                var gameDetailPage = new GameDetail(clickedGame, currentUser);
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
            if (SearchTextBoxCus.Text == "Search name game")
            {
                SearchTextBoxCus.Text = string.Empty;
                SearchTextBoxCus.Foreground = (System.Windows.Media.Brush)Application.Current.Resources["SecondaryBrush"];
            }
        }
        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBoxCus.Text))
            {
                SearchTextBoxCus.Text = "Search name game";
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
                SessionManager.ClearSession();
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
            // Only initialize the session if it's not already initialized (to avoid redundant calls)
            if (string.IsNullOrEmpty(SessionManager.LoggedInGamerId))
            {
                SessionManager.InitializeSession(_user, _gamerRepo);
            }

            // Create an instance of FriendService
            _friendService = new FriendService(_friendRepo, _gamerRepo);

            // Pass the _user object to ProfileWindow
            ProfileWindow profileWindow = new ProfileWindow(_user, _friendService);
            profileWindow.Show();

            // Hide and close the current window
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
            string Search_input = SearchTextBoxCus.Text.Trim().ToLower();
            if (Search_input == "search name game")
            {
                Search_input = string.Empty;
            }
            if (!selectedGenre.IsNullOrEmpty())
            {
                string selectedGenresText = string.Join(", ", selectedGenre);
                MessageBox.Show($"Các genre được chọn truyen toi SEARCH button: {selectedGenresText}");
            }
            if (!selectedPub.IsNullOrEmpty())
            {
                string selectedPubsText = string.Join(", ", selectedPub);
                MessageBox.Show($"Các publisher được chọn truyen toi SEARCH button: {selectedPubsText}");
            }

            SearchWindow search = new SearchWindow(CurrentWin, Search_input, selectedGenre, selectedPub);
            search.Show();
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
    }
}