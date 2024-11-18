using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.Services;
using FLauncher.ViewModel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace FLauncher.Views
{
    /// <summary>
    /// Interaction logic for GameDetail.xaml
    /// </summary>
    public partial class GameDetail : Window
       

    {
        private Game _game;
        private Gamer _gamer;
        private GamePublisher _gamePublisher;
        private readonly INotiRepository _notiRepo;
        private readonly IFriendRepository _friendRepo;
        private readonly IGameRepository _gameRepo;
        private readonly IGenresRepository  _genreRepo;
        private readonly IReviewRepository _reviewRepo;
        private readonly IPublisherRepository _publisherRepo;
        private readonly IGamerRepository _gamerRepo;
        public GameDetail(Game game, Gamer gamer, GamePublisher gamePublisher)
        {
            InitializeComponent();
            _notiRepo = new NotiRepository();
            _friendRepo = new FriendRepository();
            _gameRepo = new GameRepository();
            _genreRepo = new GenreRepository();
            _reviewRepo = new ReviewRepository();
            _publisherRepo = new PublisherRepository();
            _gamerRepo = new GamerRepository();
            InitializeData( game,  gamer,  gamePublisher);

        }
        private async void InitializeData(Game game, Gamer gamer, GamePublisher gamePublisher)
        {
            _game = game;
          
            _gamer = gamer;
            _gamePublisher = gamePublisher;

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
                DataContext = new GameDetailViewModel(game, gamer, genres, reviews, unreadNotifications, friendInvitations, publisher, updates, friendwithsamegame, UnlockAchivements, Achivements, LockAchivements, Unlock, reviewers);
            }
            else if (_gamePublisher != null)
            {
                DataContext = new GameDetailViewModel(game, genres, reviews, publisher, updates);
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
         
            _gameRepo.Download_game(_game , location, _gamer); 
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {

            _gameRepo.Play_Game(_game , _gamer);
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
                _gameRepo.Upload_game(_gamePublisher,_game ,selectedFilePath, message);
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

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Search the store")
            {
                SearchTextBox.Text = string.Empty;
                SearchTextBox.Foreground = (System.Windows.Media.Brush)System.Windows.Application.Current.Resources["SecondaryBrush"];
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Search the store";
            }
        }

    }
}
