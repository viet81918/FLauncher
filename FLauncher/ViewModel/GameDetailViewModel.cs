using FLauncher.DAO;
using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.ViewModel
{
    public class GameDetailViewModel : INotifyPropertyChanged
    {
        private GamePublisher _gamePublisher;

        public GamePublisher GamePublisher
        {
            get => _gamePublisher;
            set
            {
                if (_gamePublisher != value)
                {
                    _gamePublisher = value;
                    OnPropertyChanged(nameof(GamePublisher));
                }
            }
        }

        public Game Game { get; }
        public Gamer Gamer { get; }
        public ObservableCollection<Genre> Genres { get; }
        public ObservableCollection<Review> Reviews { get; }
        public int UnreadNotificationCount => UnreadNotifications?.Count ?? 0;
        public ObservableCollection<Notification> UnreadNotifications { get; }
        public ObservableCollection<Achivement>  Achivement { get; }
        public ObservableCollection<Achivement> UnlockAchivement { get; }
        public ObservableCollection<UnlockAchivement> Unlock { get; }
        public ObservableCollection<Achivement> LockAchivement { get; }
        public int FriendInvitationCount => FriendInvitations?.Count ?? 0;
        public ObservableCollection<Friend> FriendInvitations { get; }
        public double AverageRating => Reviews?.Any() == true ? Reviews.Average(r => r.Rating) : 0;
        public ObservableCollection<Update> Updates { get; }
        public string Name => Gamer?.Name ?? GamePublisher?.Name;
        public double Money => Gamer?.Money ?? GamePublisher?.Money ?? 0.0;
        public ObservableCollection<Gamer> Friendwiththesamegame { get; }
        public ObservableCollection<UnlockAchivementViewModel> UnlockAchivementViewModels { get; set; }
        public ObservableCollection<ReviewGamerViewModel> ReviewGamerViewModels { get; set; }
        public ObservableCollection<Gamer> Reviewers { get; }
        public bool IsGamer { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsBuy { get; set; }
        public bool IsPublished { get; set; }
        public GameDetailViewModel(Game game, Gamer gamer, IEnumerable<Genre> genres, IEnumerable<Review> reviews, IEnumerable<Notification> unreadNotifications, IEnumerable<Friend> friendInvitations, GamePublisher publisher, IEnumerable<Update> updates, IEnumerable<Gamer> friendwiththesamegame, IEnumerable<Achivement> UnlockAchivements, IEnumerable<Achivement> Achivements, IEnumerable<Achivement> LockAchivements, IEnumerable<UnlockAchivement>  unlockAchivementsData, IEnumerable<Gamer>  reviewers, bool isBuy, bool isPublished)
        {
            IsGamer = true;
            IsPublisher = false;
            IsPublished = isPublished;
            IsBuy = isBuy;
            Game = game;
            Gamer = gamer;
            Genres = new ObservableCollection<Genre>(genres);
            Reviews = new ObservableCollection<Review>(reviews);
            UnreadNotifications = new ObservableCollection<Notification>(unreadNotifications);
            FriendInvitations = new ObservableCollection<Friend>(friendInvitations);
            Updates = new ObservableCollection<Update>(updates);
            Friendwiththesamegame = new ObservableCollection<Gamer>(friendwiththesamegame);
            UnlockAchivement = new ObservableCollection<Achivement>(UnlockAchivements);
            Achivement = new ObservableCollection<Achivement>(Achivements);
            LockAchivement = new ObservableCollection<Achivement>(LockAchivements);
            // Load the GamePublisher asynchronously
            // Tạo danh sách ViewModel cho UnlockAchivements
            UnlockAchivementViewModels = new ObservableCollection<UnlockAchivementViewModel>();

            foreach (var unlock in unlockAchivementsData)
            {
                var achivement = UnlockAchivements.FirstOrDefault(a => a.AchivementId == unlock.AchievementId && a.GameId == unlock.GameId);
                if (achivement != null)
                {
                    UnlockAchivementViewModels.Add(new UnlockAchivementViewModel
                    {
                        Name = achivement.Name,
                        UnlockImageLink = achivement.UnlockImageLink,
                        DateUnlockString = unlock.DateUnlockString,
                        AchivmentId = unlock.AchievementId,
                        GameId = unlock.GameId,
                        GamerId = unlock.GamerId
                    });
                }
            }
            ReviewGamerViewModels = new ObservableCollection<ReviewGamerViewModel>();
            foreach (var review in reviews)
            {
                var reviewer = reviewers.FirstOrDefault(a => a.GamerId == review.GamerId);
                if (reviewer != null)
                {
                    ReviewGamerViewModels.Add(new ReviewGamerViewModel
                    {
                        Name = reviewer.Name,
                        AvatarLink = reviewer.AvatarLink,
                        Rating = review.Rating,
                        Description = review.Description
                    });
                }
            }
            LoadGamePublisher(game);

        }

        public GameDetailViewModel(Game game, IEnumerable<Genre> genres, IEnumerable<Review> reviews,GamePublisher GamePublisher, IEnumerable<Update> updates)
        {
            Game = game;
            Genres = new ObservableCollection<Genre>(genres);
            Reviews = new ObservableCollection<Review>(reviews);
            Updates = new ObservableCollection<Update>(updates);
            _gamePublisher = GamePublisher;
            // Load the GamePublisher asynchronously
            LoadGamePublisher(game);
        }

        // Asynchronous method to load GamePublisher
        private async void LoadGamePublisher(Game game)
        {
            // Assuming GetPublisherByGame is a method that returns Task<GamePublisher>
            GamePublisher = await PublisherDAO.Instance.GetPublisherByGame(game);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
