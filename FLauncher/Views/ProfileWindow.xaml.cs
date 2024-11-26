using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.Services;
using FLauncher.ViewModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace FLauncher.Views
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private readonly Gamer _currentGamer;
        private readonly FriendService _friendService;

        private ProfileWindowViewModel _viewModel;
        private readonly GamerRepository _gamerRepo;
        private readonly FriendRepository _friendRepo;



        private Model.User _user;
        private readonly UserRepository _userRepo;

        public ProfileWindow(Gamer gamer, FriendService friendService)
        {
            Debug.WriteLine("ProfileWindow constructor called.");

            InitializeComponent();
            _userRepo = new UserRepository();
            _user = _userRepo.GetUserByGamer(gamer);
            if (_user.Role == 2) // Giả sử 1 là Publisher
            {
                MessageButon.Visibility = Visibility.Collapsed; // Ẩn
            }
            else if (_user.Role == 3) // Giả sử 2 là Gamer
            {
                MessageButon.Visibility = Visibility.Visible; // Hiện
            }
            _userRepo = new UserRepository();
            
            _currentGamer = gamer;
            _friendService = friendService;
            _friendRepo = new FriendRepository();
            _gamerRepo = new GamerRepository();



            // Call the async method after the window is initialized
            InitializeProfileDataAsync();
        }

        // Create a new async method to load data
        private async void InitializeProfileDataAsync()
        {
            var friendsList = await _friendRepo.GetListFriendForGamer(_currentGamer.GamerId);

            _viewModel = new ProfileWindowViewModel(_friendRepo, _gamerRepo, friendsList);

            DataContext = _viewModel;
            Debug.WriteLine("DataContext set.");

            await _viewModel.LoadProfileData(_currentGamer.GamerId);

            Debug.WriteLine("Load methods called.");
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
            // Prompt user to enter the Gamer ID they want to add as a friend
            string acceptId = Microsoft.VisualBasic.Interaction.InputBox("Enter the ID of the gamer you want to add as a friend:", "Add Friend");

            if (string.IsNullOrWhiteSpace(acceptId))
            {
                MessageBox.Show("Please enter a valid Gamer ID.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check if the users are already friends
            bool areAlreadyFriends = _friendService.AreGamersAlreadyFriends(_currentGamer.GamerId, acceptId);

            if (areAlreadyFriends)
            {
                MessageBox.Show("You are already friends with this gamer.", "Already Friends", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Check if there is already a pending invitation between the two gamers
            var pendingInvitations = await _friendService.GetPendingInvitations(_currentGamer.GamerId);

            bool isInvitationPending = pendingInvitations.Any(invitation =>
                (invitation.RequestId == _currentGamer.GamerId && invitation.AcceptId == acceptId) ||
                (invitation.RequestId == acceptId && invitation.AcceptId == _currentGamer.GamerId));

            if (isInvitationPending)
            {
                MessageBox.Show("You have already sent a friend request to this gamer.", "Invitation Pending", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Attempt to send the friend request
            bool success = _friendService.SendFriendRequest(_currentGamer.GamerId, acceptId);

            if (success)
            {
                MessageBox.Show("Friend request sent successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh the friend count for the current gamer
                await _viewModel.RefreshFriendCount(_currentGamer.GamerId);
            }
            else
            {
                MessageBox.Show("Friend request already exists or could not be sent.", "Request Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void InvitationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Fetch the pending invitations synchronously
                var invitations = await _friendService.GetPendingInvitations(_currentGamer.GamerId);

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
            await _viewModel.RefreshFriendCount(_currentGamer.GamerId);
        }

        private async void DenyButton_Click(object sender, RoutedEventArgs e)
        {
            var invitation = (Friend)((Button)sender).DataContext;

            // Update the invitation status to denied in the database
            await _friendService.DeclineFriendRequest(invitation.RequestId, invitation.AcceptId);

            // Refresh the invitations list
            await RefreshInvitationsList();
        }

        private async Task RefreshInvitationsList()
        {
            // Fetch the updated list of invitations
            var invitations = await _friendService.GetPendingInvitations(_currentGamer.GamerId);

            // Update the ListBox with the new list
            InvitationsListBox.ItemsSource = invitations;
        }
        private void Home_Click(object sender, MouseButtonEventArgs e)
        {
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
        private void messageButton_Click(Object sender, MouseButtonEventArgs e)
        {
            var messWindow = new MessageWindow(_currentGamer);
            messWindow.Show();
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

            MyGame myGameWindow = new MyGame(_user);
            myGameWindow.Show();
            this.Hide();
            this.Close();
        }
    }
}
