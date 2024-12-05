using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.Services;
using FLauncher.ViewModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Google.Apis.Drive.v3.Data;
using FLauncher.Utilities;
using System.Xml.Linq;
using Microsoft.IdentityModel.Tokens;
using static Azure.Core.HttpHeader;
using FLauncher.DAO;
using System.Windows.Threading;

namespace FLauncher.Views
{
    public partial class ProfileWindow : Window
    {
        
        private Gamer _gamer;
        private Game _game = null;
        private readonly FriendService _friendService;

        private ProfileWindowViewModel _viewModel;
        private readonly GamerRepository _gamerRepo;
        private readonly FriendRepository _friendRepo;
        private GamePublisher _gamePublisher;
        private readonly IPublisherRepository _publisherRepo;
        private Model.User _user;
        private readonly UserRepository _userRepo;
        private readonly IGameRepository _gameRepo;
        

        public ProfileWindow(Model.User user, FriendService friendService, Gamer friend = null)
        {
            InitializeComponent();
            _userRepo = new UserRepository();
            _friendService = friendService;
            _friendRepo = new FriendRepository();
            _gamerRepo = new GamerRepository();
            _gameRepo = new GameRepository();
            _publisherRepo = new PublisherRepository();
            _user = user;
           

            // Check if we are viewing a friend's profile
            if (friend != null)
            {
                InitializeFriendProfileDataAsync(friend);
            }
            else
            {
                 InitializeProfileDataAsync(user);
            }
          
        }


        // Load data for the current user
        private async void InitializeProfileDataAsync(Model.User user)
        {
            try
            {
                Debug.WriteLine("Logged-in user (from session): " + SessionManager.LoggedInGamerId);

                // Get the logged-in gamer and their friends
                _gamer = _gamerRepo.GetGamerByUser(user);
                var friendsList = await _friendRepo.GetListFriendForGamer(_gamer.GamerId);

                // Retrieve all games purchased by the gamer
                var games = await _gameRepo.GetGamesByGamer(_gamer);

                // Initialize collections for achievements and unlocks
                List<Achivement> allAchievements = new List<Achivement>();
                List<UnlockAchivement> allUnlocks = new List<UnlockAchivement>();

                if (games != null && games.Any())
                {
                    Debug.WriteLine($"Found {games.Count()} purchased games. Loading achievements...");

                    foreach (var game in games)
                    {
                        Debug.WriteLine($"Processing game: {game.Name}");

                        // Retrieve achievements for the current game
                        var achievements = await _gameRepo.GetAchivesFromGame(game);

                        if (achievements != null && achievements.Any())
                        {
                            allAchievements.AddRange(achievements);

                            // Retrieve unlocks for the current game's achievements
                            var unlocks = await _gameRepo.GetUnlockAchivementsFromGame(achievements, _gamer);

                            if (unlocks != null && unlocks.Any())
                            {
                                allUnlocks.AddRange(unlocks);
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"No achievements found for game: {game.Name}");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("No games found for the gamer.");
                }

                // Retrieve unlocked achievement details
                var allUnlockedAchievements = allUnlocks != null && allUnlocks.Any()
                    ? await _gameRepo.GetAchivementsFromUnlocks(allUnlocks)
                    : new List<Achivement>();

                // Retrieve games with hours and last played date
                var gameWithHours = await _gameRepo.GetGamesWithPlayingHoursAndLastPlayed(_gamer.GamerId) ?? Enumerable.Empty<(Game Game, double TotalHours, DateTime LastPlayed)>();


                // Create and set the ViewModel
                _viewModel = new ProfileWindowViewModel(_gamer, _friendRepo, _gamerRepo, friendsList, allUnlockedAchievements, allUnlocks, gameWithHours)
                {
                    IsCurrentUser = SessionManager.LoggedInGamerId == _gamer.GamerId // Is it the current user?
                };

                DataContext = _viewModel;

                await _viewModel.LoadProfileData(user);

                Debug.WriteLine("User profile data loaded.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing profile data: {ex.Message}");
            }
        }

        // Load data for a friend's profile
        private async void InitializeFriendProfileDataAsync(Gamer friend)
        {
            try
            {
                Debug.WriteLine("Logged-in user (from session): " + SessionManager.LoggedInGamerId);

                // Retrieve the friend's list of friends
                var friendsList = await _friendRepo.GetListFriendForGamer(friend.GamerId);

                // Retrieve all games purchased by the friend
                var games = await _gameRepo.GetGamesByGamer(friend);

                // Initialize collections for achievements and unlocks
                List<Achivement> allAchievements = new List<Achivement>();
                List<UnlockAchivement> allUnlocks = new List<UnlockAchivement>();

                if (games.Any())
                {
                    Debug.WriteLine($"Found purchased games. Loading achievements...");

                    foreach (var game in games)
                    {
                        Debug.WriteLine($"Processing game: {game.Name}");

                        // Retrieve achievements for the current game
                        var achievements = await _gameRepo.GetAchivesFromGame(game);

                        if (achievements != null && achievements.Any())
                        {
                            allAchievements.AddRange(achievements);

                            // Retrieve unlocks for the current game's achievements
                            var unlocks = await _gameRepo.GetUnlockAchivementsFromGame(achievements, friend);

                            if (unlocks != null && unlocks.Any())
                            {
                                allUnlocks.AddRange(unlocks);
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"No achievements found for game: {game.Name}");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("No games found for the friend.");
                }

                // Retrieve unlocked achievement details
                var allUnlockedAchievements = allUnlocks.Any()
                    ? await _gameRepo.GetAchivementsFromUnlocks(allUnlocks)
                    : new List<Achivement>();

                // Retrieve games with hours and last played date
                var gameWithHours = await _gameRepo.GetGamesWithPlayingHoursAndLastPlayed(friend.GamerId)
                                    ?? Enumerable.Empty<(Game Game, double TotalHours, DateTime LastPlayed)>();

                // Create and set the ViewModel
                _viewModel = new ProfileWindowViewModel(friend, _friendRepo, _gamerRepo, friendsList, allUnlockedAchievements, allUnlocks, gameWithHours)
                {
                    IsCurrentUser = SessionManager.LoggedInGamerId == friend.GamerId // Compare against the logged-in user ID
                };

                DataContext = _viewModel;

                // Load additional profile data
                await _viewModel.LoadFriendStatusAsync(SessionManager.LoggedInGamerId); // Use the logged-in user's ID
                _user = _userRepo.GetUserByGamer(friend);
                await _viewModel.LoadProfileData(_user);

                Debug.WriteLine("Friend profile data loaded.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing friend profile data: {ex.Message}");
            }
        }

      


        private void OnFriendSelected(object sender, RoutedEventArgs e)
        {
            var selectedFriend = (sender as FrameworkElement)?.DataContext as Gamer;

            if (selectedFriend != null)
            {
                var friendProfileWindow = new ProfileWindow(_user, _friendService, selectedFriend);
                friendProfileWindow.Show();
                this.Hide();
                this.Close();
            }
        }



        private void Polygon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // To move the window on mouse down
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            // First detect if windows is in normal state or maximized
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the App
            Close();
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

        private async void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            _gamer = _gamerRepo.GetGamerById(_user.ID);

            
            if (_gamer == null)
            {
                MessageBox.Show("Could not find your gamer profile. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Prompt user to enter the Gamer ID they want to add as a friend
            string acceptId = Microsoft.VisualBasic.Interaction.InputBox("Enter the ID of the gamer you want to add as a friend:", "Add Friend");

            if (string.IsNullOrWhiteSpace(acceptId))
            {
                MessageBox.Show("Please enter a valid Gamer ID.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check if the users are already friends
            bool areAlreadyFriends = _friendService.AreGamersAlreadyFriends(_gamer.GamerId, acceptId);
            Debug.WriteLine($"Checking if gamers are already friends: RequestId={_gamer.GamerId}, AcceptId={acceptId}");
            Debug.WriteLine($"Result: {areAlreadyFriends}");

            if (areAlreadyFriends)
            {
                MessageBox.Show("You are already friends with this gamer.", "Already Friends", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Check if there is already a pending invitation between the two gamers
            var pendingInvitations = await _friendService.GetPendingInvitations(_gamer.GamerId);
            Debug.WriteLine($"Pending Invitations for {_gamer.GamerId}: {string.Join(", ", pendingInvitations.Select(x => $"{x.RequestId}->{x.AcceptId}"))}");

            bool isInvitationPending = pendingInvitations.Any(invitation =>
                (invitation.RequestId == _gamer.GamerId && invitation.AcceptId == acceptId) ||
                (invitation.RequestId == acceptId && invitation.AcceptId == _gamer.GamerId));

            if (isInvitationPending)
            {
                MessageBox.Show("You have already sent a friend request to this gamer.", "Invitation Pending", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Attempt to send the friend request
            bool success = await _friendService.SendFriendRequest(_gamer.GamerId, acceptId);

            if (success)
            {
                MessageBox.Show("Friend request sent successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh the friend count for the current gamer
                await _viewModel.RefreshFriendCount(_gamer.GamerId);
            }
            else
            {
                MessageBox.Show("Friend request already exists or could not be sent.", "Request Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ReAddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            _gamer = _gamerRepo.GetGamerById(_user.ID);

            if (_gamer == null)
            {
                MessageBox.Show("Could not find your gamer profile. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Assuming the current profile being viewed has a GamerId accessible via a property or binding
            string requestId = SessionManager.LoggedInGamerId;

            if (string.IsNullOrWhiteSpace(requestId))
            {
                MessageBox.Show("Could not determine the profile to re-add as a friend.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check if the users are already friends (should not be, since it's a re-add)
            bool areAlreadyFriends = _friendService.AreGamersAlreadyFriends(requestId,_gamer.GamerId);

            if (areAlreadyFriends)
            {
                MessageBox.Show("You are already friends with this gamer.", "Already Friends", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Attempt to send the friend request
            bool success = await _friendService.SendFriendRequest(requestId,_gamer.GamerId);

            if (success)
            {
                MessageBox.Show("Friend request sent successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Optionally refresh the profile UI or friend list
                await _viewModel.RefreshFriendCount(_gamer.GamerId);
            }
            else
            {
                MessageBox.Show("Friend request could not be sent.", "Request Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void InvitationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Fetch the pending invitations synchronously
                var invitations = await _friendService.GetPendingInvitations(_gamer.GamerId);

                if (invitations != null && invitations.Count > 0)
                {
                    InvitationsListBox.ItemsSource = invitations;
                    InvitationsListBox.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("No pending invitations found.", "No Invitations", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (ArgumentNullException ex)
            {
                // Handle the exception, e.g., log it or display an error message
                MessageBox.Show($"Error: {ex.Message}", "Gamer Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            var invitation = (Friend)((Button)sender).DataContext;

            // Update the invitation status to accepted in the database
            await _friendService.AcceptFriendRequest(invitation.RequestId, invitation.AcceptId);

            // Refresh the invitations list
            await RefreshInvitationsList();

            await _viewModel.RefreshFriendCount(_gamer.GamerId);

            // Refresh the friend list in the ProfileWindow
            await RefreshFriendListAsync();
        }

        private async void DenyButton_Click(object sender, RoutedEventArgs e)
        {
            var invitation = (Friend)((Button)sender).DataContext;

            // Update the invitation status to denied in the database
            await _friendService.DeclineFriendRequest(invitation.RequestId, invitation.AcceptId);

            // Refresh the invitations list
            await RefreshInvitationsList();
        }

        public async Task RefreshFriendListAsync()
        {
            // Retrieve the updated list of friends for the current gamer
            var updatedFriendsList = await _friendRepo.GetListFriendForGamer(_gamer.GamerId);

            // Update the ViewModel's friends list
            _viewModel.UpdateFriendsList(updatedFriendsList);

            Debug.WriteLine("Friend list refreshed.");
        }


        private async Task RefreshInvitationsList()
        {
            // Fetch the updated list of invitations
            var invitations = await _friendService.GetPendingInvitations(_gamer.GamerId);

            // Update the ListBox with the new list
            InvitationsListBox.ItemsSource = invitations;
        }


        private async void UnfriendButton_Click(object sender, RoutedEventArgs e)
        {
            // Make sure you have the correct friend information (the one you want to unfriend)
            string currentUserId = SessionManager.LoggedInGamerId;  // The current user's GamerId
            string friendUserId = _user.ID;  // The friend's ID

            // Call the UnfriendAsync method to remove the friendship from the database
            await _friendRepo.Unfriend(currentUserId, friendUserId);

            await _viewModel.RefreshFriendCount(SessionManager.LoggedInGamerId);

            // Refresh the friend list in the ProfileWindow
            await RefreshFriendListAsync();

            _viewModel.IsFriend = false;
            

            
            // Refresh the UI by reloading the profile data
             InitializeProfileDataAsync(_user);
        }
     

        private void Home_Click(object sender, MouseButtonEventArgs e)
        {
            _gamer = _gamerRepo.GetGamerById(SessionManager.LoggedInGamerId);
            _user = _userRepo.GetUserByGamer(_gamer);
            CustomerWindow cus = new CustomerWindow(_user);
            cus.Show();
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

            if (System.IO.File.Exists(jsonFilePath))
            {
                System.IO.File.Delete(jsonFilePath);
            }
        }
        private void messageButton_Click(object sender, RoutedEventArgs e)
        {
            // Create and show the MessageWindow
            _gamer = _gamerRepo.GetGamerById(SessionManager.LoggedInGamerId);
            var friend = _gamerRepo.GetGamerByUser(_user);
            MessageWindow messageWindow = new MessageWindow(_gamer, friend);
            messageWindow.Show();
            this.Hide();
            this.Close();
        }
        private void searchButton_Click(object sendedr, MouseButtonEventArgs e)
        {
            _gamer = _gamerRepo.GetGamerById(SessionManager.LoggedInGamerId);
            _user = _userRepo.GetUserByGamer(_gamer);
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
            _gamer = _gamerRepo.GetGamerById(SessionManager.LoggedInGamerId);
            _user = _userRepo.GetUserByGamer(_gamer);
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
        private void MyGame_Click(object sender, RoutedEventArgs e)
        {
            _gamer = _gamerRepo.GetGamerById(SessionManager.LoggedInGamerId);
            _user = _userRepo.GetUserByGamer(_gamer);
            MyGame myGameWindow = new MyGame(_user);
            myGameWindow.Show();
            this.Hide();
            this.Close();
        }
        private async void GameName_Selected(object sender, MouseButtonEventArgs e)
        {
            var gameSelected = sender as TextBlock;
            if (gameSelected == null)
            {
                MessageBox.Show("Sự kiện không được gửi từ TextBlock!");
                return;
            }
            var gameData = gameSelected.DataContext as TrackingMyGameViewModel;
            if (gameData == null) return;
            string gameName = gameData.GameName;
            var GameN = await _gameRepo.GetGameByName(gameName);
            Game _gameN = GameN as Game;
            GameDetail gameDTW = new GameDetail(_gameN, _user);           
            this.Hide();
            gameDTW.Show();
            this.Close();
        }
    }
}
