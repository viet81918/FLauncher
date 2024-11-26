using FLauncher.DAO;
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
    /// Interaction logic for AchivementManagement.xaml
    /// </summary>
    public partial class AchivementManagement : Window
    {
        private Game _game;
        private User _user;
        private readonly IGameRepository _gameRepo;
        public AchivementManagement(User user , Game game)
        {
            _user = user;
            _game = game;

            InitializeComponent();
            _gameRepo = new GameRepository();
            var sampleAchievements =  _gameRepo.GetAchivesFromGame(game).Result;
            // Set the DataContext
            DataContext = new AchivementViewModel(sampleAchievements);
        }
        private void AchievementSelected(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Achivement achievement)
            {
                var viewModel = DataContext as AchivementViewModel;
                if (viewModel != null)
                {
                    viewModel.SelectedAchievement = achievement;
                }
            }
        }

        private async void EditAchievementButton_Click(object sender, RoutedEventArgs e)
        {
            // Step 1: Get the selected achievement from the ViewModel
            var viewModel = DataContext as AchivementViewModel;
            if (viewModel == null || viewModel.SelectedAchievement == null)
            {
                MessageBox.Show("Please select an achievement to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Step 2: Open the Edit Achievement window with the selected achievement data
            var editAchievementWindow = new AddAchievementWindow
            {
                Achievement = new Achivement
                {
                    Id = viewModel.SelectedAchievement.Id,
                    AchivementId = viewModel.SelectedAchievement.AchivementId,
                    Name = viewModel.SelectedAchievement.Name,
                    Description = viewModel.SelectedAchievement.Description,
                    Trigger = viewModel.SelectedAchievement.Trigger,
                    UnlockImageLink = viewModel.SelectedAchievement.UnlockImageLink,
                    LockImageLink = viewModel.SelectedAchievement.LockImageLink,
                    GameId = viewModel.SelectedAchievement.GameId
                },
                _isEditMode = true // Set the mode to edit
            };

            if (editAchievementWindow.ShowDialog() == true)
            {
                // Step 3: Retrieve the edited achievement data
                var editedAchievement = editAchievementWindow.Achievement;

                // Step 4: Update the achievement in the database
                var updatedAchievement = await _gameRepo.UpdateAchievement(
                    idobject: editedAchievement.Id,
                    id: editedAchievement.AchivementId,
                    gameid: editedAchievement.GameId,
                    trigger: editedAchievement.Trigger,
                    description: editedAchievement.Description,
                    name: editedAchievement.Name,
                    unlockImagePath: editedAchievement.UnlockImageLink,
                    lockImagePath: editedAchievement.LockImageLink,
                    achievement: viewModel.SelectedAchievement
                );

                if (updatedAchievement != null)
                {
                    // Step 5: Update the achievement in the ViewModel's collection
                    var existingAchievement = viewModel.Achievements.FirstOrDefault(a => a.Id == updatedAchievement.Id);
                    if (existingAchievement != null)
                    {
                        // Update properties of the existing achievement
                        existingAchievement.AchivementId = updatedAchievement.AchivementId;
                        existingAchievement.Name = updatedAchievement.Name;
                        existingAchievement.Description = updatedAchievement.Description;
                        existingAchievement.Trigger = updatedAchievement.Trigger;
                        existingAchievement.UnlockImageLink = updatedAchievement.UnlockImageLink;
                        existingAchievement.LockImageLink = updatedAchievement.LockImageLink;

                        // Notify WPF about the changes
                        viewModel.SelectedAchievement = null; // Clear selection briefly to refresh bindings
                        viewModel.SelectedAchievement = existingAchievement; // Re-select the updated item
                    }


                    MessageBox.Show("Achievement updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }


        private async void AddAchievementButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the Add Achievement window
            var addAchievementWindow = new AddAchievementWindow();
            if (addAchievementWindow.ShowDialog() == true)
            {
                // Retrieve the new achievement data
                var newAchievement = addAchievementWindow.Achievement;

                // Add the new achievement to the ViewModel's collection
                var viewModel = DataContext as AchivementViewModel;
                if (viewModel != null)
                {
                    // Add to local ObservableCollection
                 

                    // Save to database
                    var achivement =  await SaveAchievementToDatabaseAsync(newAchievement);
                    viewModel.Achievements.Add(achivement);

                }
            }
        }
        private async void DeleteAchievementButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the Achievement object from the button's DataContext
            if (sender is Button button && button.DataContext is Achivement achievement)
            {
                // Check if the Achievement has a valid Id (primary key)
                if (achievement.AchivementId == null)
                {
                    MessageBox.Show("The achievement does not have a valid ID. Cannot delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Exit early if no valid ID is found
                }

                // Reset the selected achievement before deletion
                if (DataContext is AchivementViewModel viewModel)
                {
                    // Reset selected achievement
                    viewModel.SelectedAchievement = null;

                    // Confirm deletion
                    var result = MessageBox.Show($"Are you sure you want to delete the achievement '{achievement.Name}'?",
                                                 "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            // Clear the image sources for the specific achievement being deleted
                            var unlock = achievement.UnlockImageLink;
                            var Lock = achievement.LockImageLink;
                            achievement.LockImageLink = null;  // Nullify the image source
                            achievement.UnlockImageLink = null;  // Nullify the image source

                            // Trigger garbage collection to release any unused resources (optional)
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            achievement.UnlockImageLink = unlock;
                            achievement.LockImageLink = Lock;

                            // Remove from the database
                            await _gameRepo.DeleteAchievement(achievement);

                            // Remove from the ObservableCollection
                            viewModel.Achievements.Remove(achievement);

                            // Inform the user of success
                            MessageBox.Show("Achievement deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            // Display error if something goes wrong
                            MessageBox.Show($"Error deleting achievement: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Unable to find the ViewModel.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private async Task<Achivement> SaveAchievementToDatabaseAsync(Achivement achievement)
        {
            try
            {
                // Call the data access layer to save the achievement
                var Achivement =  await _gameRepo.AddAchivement(
                    achievement.Id,
                    achievement.AchivementId,
                    _game.GameID,
                    achievement.Trigger,
                    achievement.Description,
                    achievement.Name,
                    achievement.UnlockImageLink,
                    achievement.LockImageLink
                );

                MessageBox.Show("Achievement saved to database successfully!");
                return Achivement;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving achievement to database: {ex.Message}");
                return null;
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
        }
        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
          
        }
        private void messageButton_Click(Object sender, MouseButtonEventArgs e)

        {
            
        }
        private void logoutButton_Click(object sender, MouseButtonEventArgs e)
        {
           
        }
        private void ProfileIcon_Click(object sender, MouseButtonEventArgs e)
        {
         
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
          
        }
        private void Home_Click(object sender, MouseButtonEventArgs e)
        {
            CustomerWindow cus = new CustomerWindow(_user);
            cus.Show();
            this.Hide();
            this.Close();
        }
    }
}
