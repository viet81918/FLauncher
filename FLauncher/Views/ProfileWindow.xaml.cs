using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.Services;
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
using System.Windows.Shapes;

namespace FLauncher.Views
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private readonly Gamer _currentGamer;
        private readonly FriendService _friendService;
        private readonly ProfileWindowViewModel _viewModel;
        
        public ProfileWindow(Gamer gamer, FriendService friendService)
        {
            InitializeComponent();
            _currentGamer = gamer;
            _friendService = friendService;
            DataContext = gamer;
            // Set up the ViewModel with the appropriate repositories
            _viewModel = new ProfileWindowViewModel(new FriendRepository(), new GamerRepository());

            // Load friend count
            _viewModel.LoadProfileData(_currentGamer.GamerId);

            this.DataContext = _viewModel;
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
                _viewModel.RefreshFriendCount(_currentGamer.GamerId);
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
                var invitations = await _friendService.GetPendingInvitations(_currentGamer.GamerId);  // Blocking call

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
    }
}
