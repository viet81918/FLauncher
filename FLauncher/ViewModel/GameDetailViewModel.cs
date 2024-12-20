﻿using FLauncher.DAO;
using FLauncher.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

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
        private ObservableCollection<Genre> _genres;
        public ObservableCollection<Genre> Genres
        {
            get => _genres;
            set
            {
                if (_genres != value)
                {
                    _genres = value;
                    OnPropertyChanged(nameof(Genres));
                }
            }
        }

        public ObservableCollection<Review> Reviews { get; }
        public int UnreadNotificationCount => UnreadNotifications?.Count ?? 0;
        public ObservableCollection<Notification> UnreadNotifications { get; }
        public ObservableCollection<Achivement> Achivement { get; }
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
        public ObservableCollection<Gamer> Gamers { get; }
        public bool IsGamer { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsBuy { get; set; }
        public bool IsNotBuy { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDownload { get; set; }
        public bool IsNotDown { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsNotUpdate { get; set; }
        public GameDetailViewModel(Game game, Gamer gamer, IEnumerable<Genre> genres, IEnumerable<Review> reviews, IEnumerable<Notification> unreadNotifications, IEnumerable<Friend> friendInvitations, GamePublisher publisher, IEnumerable<Update> updates, IEnumerable<Gamer> friendwiththesamegame, IEnumerable<Achivement> UnlockAchivements, IEnumerable<Achivement> Achivements, IEnumerable<Achivement> LockAchivements, IEnumerable<UnlockAchivement> unlockAchivementsData, IEnumerable<Gamer> reviewers, bool isBuy, bool isDownload, bool isUpdate)
        {
            IsGamer = true;
            IsPublisher = false;
            IsBuy = isBuy;

            if (IsBuy == true)
            {
                IsNotBuy = false;
            }
            else
            {
                IsNotBuy = true;
            }
            IsDownload = isDownload;
            if (IsDownload == true)
            {
                IsNotDown = false;
            }
            else
            {
                IsNotDown = true;
            }
            IsUpdate = isUpdate;
            if (IsUpdate == true)
            {
                IsNotUpdate = false;
            }
            else
            {
                IsNotUpdate = true;
            }
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
                        Description = achivement.Description,
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

        public GameDetailViewModel(Game game, IEnumerable<Genre> genres, IEnumerable<Review> reviews, GamePublisher GamePublisher, IEnumerable<Update> updates, bool isPublished, IEnumerable<Achivement> Achivements, IEnumerable<Gamer> reviewers)
        {
            IsGamer = false;
            IsPublisher = true;
            IsPublished = isPublished;

            Game = game;
            Genres = new ObservableCollection<Genre>(genres);
            Reviews = new ObservableCollection<Review>(reviews);
            Updates = new ObservableCollection<Update>(updates);
            Gamers = new ObservableCollection<Gamer>(reviewers);
            _gamePublisher = GamePublisher;
            Achivement = new ObservableCollection<Achivement>(Achivements);
            // Load the GamePublisher asynchronously
            LoadGamePublisher(game);
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

        }

        // Asynchronous method to load GamePublisher
        private async void LoadGamePublisher(Game game)
        {
            // Assuming GetPublisherByGame is a method that returns Task<GamePublisher>
            GamePublisher = await PublisherDAO.Instance.GetPublisherByGame(game);
        }

        #region INotifyPropertyChanged implimentation
        public event PropertyChangedEventHandler PropertyChanged;
        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}
