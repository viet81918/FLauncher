using FLauncher.DAO;
using FLauncher.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FLauncher.ViewModel
{
    public class MyGameViewModel : INotifyPropertyChanged
    {
        private readonly CategoryDAO _categoryDAO; // Injecting CategoryDAO

        public Gamer Gamer { get; }

        public int UnreadNotificationCount => UnreadNotifications?.Count ?? 0;
        public ObservableCollection<Notification> UnreadNotifications { get; }

        public int FriendInvitationCount => FriendInvitations?.Count ?? 0;
        public ObservableCollection<Friend> FriendInvitations { get; }

        public string Name => Gamer?.Name;
        public double Money => Gamer?.Money ?? 0.0;
        public ObservableCollection<Game> MyGames { get; }

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

        public ObservableCollection<Achivement> Achivement { get; }
        public ObservableCollection<Achivement> UnlockAchivement { get; }
        public ObservableCollection<UnlockAchivement> Unlock { get; }
        public ObservableCollection<Achivement> LockAchivement { get; }

        public double AverageRating => Reviews?.Any() == true ? Reviews.Average(r => r.Rating) : 0;
        public ObservableCollection<Update> Updates { get; }

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

        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();



        public MyGameViewModel(CategoryDAO categoryDAO, Gamer gamer, IEnumerable<Notification> unreadNotifications, IEnumerable<Friend> friendInvitations, IEnumerable<Game> myGames)
        {
            _categoryDAO = categoryDAO;
            Gamer = gamer;
            UnreadNotifications = new ObservableCollection<Notification>(unreadNotifications);
            FriendInvitations = new ObservableCollection<Friend>(friendInvitations);
            MyGames = new ObservableCollection<Game>(myGames);

            LoadCategories();
        }

        public MyGameViewModel(Game game, Gamer gamer, IEnumerable<Genre> genres, IEnumerable<Review> reviews, IEnumerable<Notification> unreadNotifications, IEnumerable<Friend> friendInvitations, GamePublisher publisher, IEnumerable<Update> updates, IEnumerable<Gamer> friendwiththesamegame, IEnumerable<Achivement> UnlockAchivements, IEnumerable<Achivement> Achivements, IEnumerable<Achivement> LockAchivements, IEnumerable<UnlockAchivement> unlockAchivementsData, IEnumerable<Gamer> reviewers, bool isBuy, bool isDownload, bool isUpdate, IEnumerable<Game> myGames, CategoryDAO categoryDAO)
        {
            _categoryDAO = categoryDAO;
            MyGames = new ObservableCollection<Game>(myGames);
            IsGamer = true;
            IsPublisher = false;
            IsBuy = isBuy;

            IsNotBuy = !IsBuy;
            IsDownload = isDownload;
            IsNotDown = !IsDownload;
            IsUpdate = isUpdate;
            IsNotUpdate = !IsUpdate;

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
            LoadCategories();
        }

        // Load categories from the database using CategoryDAO
        private async void LoadCategories()
        {
            var categories = await _categoryDAO.GetAllCategoriesAsync();
            Categories.Clear(); // Clear existing categories to avoid duplication.

            foreach (var category in categories)
            {
                // Fetch games for each category.
                category.GameIds = (await _categoryDAO.GetGamesByCategoryAsync(category.NameCategories))
                                    .Select(g => g.GameID)
                                    .ToList();

                Categories.Add(category); // Add the category to the ObservableCollection.
            }
        }




        // Add a new category
        public async Task AddCategoryAsync(string categoryName)
        {
            if (!string.IsNullOrWhiteSpace(categoryName) && !Categories.Any(c => c.NameCategories == categoryName))
            {
                var newCategory = new Category
                {
                    NameCategories = categoryName,
                    GamerId = Gamer?.GamerId
                };

                await _categoryDAO.AddCategoryAsync(categoryName, Gamer?.GamerId);
                Categories.Add(newCategory); // Add the new category to the ObservableCollection.
            }
        }


        // Delete a category
        public async Task DeleteCategoryAsync(string categoryName)
        {
            var category = Categories.FirstOrDefault(c => c.NameCategories == categoryName);
            if (category != null)
            {
                await _categoryDAO.DeleteCategoryAsync(categoryName);
                Categories.Remove(category); // Remove the category from the ObservableCollection.
            }
        }




        // Add a game to a category
        public async Task AddGameToCategoryAsync(string categoryName, string gameId)
        {
            var category = Categories.FirstOrDefault(c => c.NameCategories == categoryName);
            if (category != null && !category.GameIds.Contains(gameId))
            {
                await _categoryDAO.AddGameToCategoryAsync(categoryName, gameId);
                category.GameIds.Add(gameId); // Update the GameIds in the ObservableCollection.

                // Optionally: Trigger UI update if needed.
                var index = Categories.IndexOf(category);
                Categories[index] = category;
            }
        }


        // Remove a game from a category
        public async Task RemoveGameFromCategoryAsync(string categoryName, string gameId)
        {
            var category = Categories.FirstOrDefault(c => c.NameCategories == categoryName);
            if (category != null && category.GameIds.Contains(gameId))
            {
                await _categoryDAO.RemoveGameFromCategoryAsync(categoryName, gameId);
                category.GameIds.Remove(gameId); // Update the GameIds in the ObservableCollection.

                // Optionally: Trigger UI update if needed.
                var index = Categories.IndexOf(category);
                Categories[index] = category;
            }
        }


        // Load the publisher data for the game
        private async void LoadGamePublisher(Game game)
        {
            GamePublisher = await PublisherDAO.Instance.GetPublisherByGame(game);
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
