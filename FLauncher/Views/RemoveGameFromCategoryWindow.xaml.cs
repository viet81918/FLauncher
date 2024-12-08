using FLauncher.DAO;
using FLauncher.Model;
using System.Collections.ObjectModel;
using System.Windows;

namespace FLauncher.Views
{
    public partial class RemoveGameFromCategoryWindow : Window
    {
        private readonly CategoryDAO _categoryDAO;
        private ObservableCollection<Category> _categories;
        private Game _selectedGame; // This will hold the selected game

        public RemoveGameFromCategoryWindow(Game selectedGame, ObservableCollection<Category> categories, CategoryDAO categoryDAO)
        {
            InitializeComponent();

            _categoryDAO = categoryDAO;
            _selectedGame = selectedGame;
            _categories = categories;

            // Set the category for the selected game asynchronously
            SetCategoryForGame(categories);
        }

        private async void SetCategoryForGame(ObservableCollection<Category> categories)
        {
            // Check which category the selected game belongs to
            var category = await GetCategoryForGameAsync(categories);

            if (category != null)
            {
                // Only display the category the game belongs to
                _categories.Clear();
                var combinedCategories = new ObservableCollection<Category>(_categories.Concat(categories));
                _categories = combinedCategories; // Combine the original categories with the new ones  
                CategoriesListBox.ItemsSource = _categories; // Bind to the category
            }
            else
            {
                MessageBox.Show("The selected game is not found in any category.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private async Task<Category> GetCategoryForGameAsync(ObservableCollection<Category> categories)
        {
            // Loop through each category and find the one containing the selected game
            foreach (var category in _categories)
            {
                var gamesInCategory = await _categoryDAO.GetGamesByCategoryAsync(category.NameCategories);
                if (gamesInCategory.Any(game => game.GameID == _selectedGame.GameID))
                {
                    return category;
                }
            }

            return null;
        }

        // Handler for the "Remove" button
        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = CategoriesListBox.SelectedItem as string; // Get the selected category

            if (!string.IsNullOrWhiteSpace(selectedCategory) && _selectedGame != null)
            {
                // Remove the selected game from the selected category
                await _categoryDAO.RemoveGameFromCategoryAsync(selectedCategory, _selectedGame.GameID.ToString());

                MessageBox.Show($"Game '{_selectedGame.Name}' has been removed from category '{selectedCategory}'", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please select a category.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Handler for the "Cancel" button
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window without performing any action
            DialogResult = false;
            Close();
        }
    }
}
