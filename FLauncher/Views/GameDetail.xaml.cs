using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FLauncher.Views
{
    /// <summary>
    /// Interaction logic for GameDetail.xaml
    /// </summary>
    public partial class GameDetail : Window
       

    {
        private Gamer _gamer;
        private GamePublisher _gamePublisher;
        private readonly NotiRepository _notiRepo;
        private readonly FriendRepository _friendRepo;

        private readonly GenreRepository _genreRepo;
        private readonly ReviewRepository _reviewRepo;
        private readonly PublisherRepository _publisherRepo;
        public GameDetail(Game game, Gamer gamer, GamePublisher gamePublisher)
        {
            InitializeComponent();

    
            _notiRepo = new NotiRepository();
            _friendRepo = new FriendRepository();
         
            _genreRepo = new GenreRepository();
            _reviewRepo = new ReviewRepository();
            _publisherRepo = new PublisherRepository();
            _gamer = gamer;
            _gamePublisher = gamePublisher;
            
            var genres = _genreRepo.GetGenresFromGame(game); // Get genres from your repository
            var reviews = _reviewRepo.GetReviewsByGame(game); // Get reviews from your repository
            var publisher = _publisherRepo.GetPublisherByGame(game);
            var updates = _publisherRepo.getUpdatesForGame(game);

            // Set the DataContext to your ViewModel            
            if(_gamer != null)
            {
                var friendwithsamegame = _friendRepo.GetFriendWithTheSameGame(game, _gamer );
                var unreadNotifications = _notiRepo.GetUnreadNotiforGamer(_gamer);
                var friendInvitations = _friendRepo.GetFriendInvitationsForGamer(_gamer);
                DataContext = new GameDetailViewModel(game, gamer, genres, reviews, unreadNotifications, friendInvitations, publisher, updates, friendwithsamegame);
            }
            else if(_gamePublisher != null)
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

    }
}
