using FLauncher.Model;
using FLauncher.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CategorySelection.xaml
    /// </summary>
    public partial class CategorySelection : Window
    {
        private ObservableCollection<Category> _categories;
        private readonly ICategoryRepository cateRepo  ;
        private readonly IGameRepository gameRepository;
        private Game _selectedGame; // To hold the game that will be added to the category
      
        
      
        // Constructor for deleting category
        public CategorySelection(ObservableCollection<Category> categories)
        {
            InitializeComponent();
            cateRepo = new CategoryRepository();
            gameRepository = new GameRepository();  
            _categories = categories;
            CategoryListBox.ItemsSource = _categories;
            ActionButton.Content = "Delete Category";
            ActionButton.Click += DeleteCategoryButton_Click;
        }

        // Constructor for adding a game to a category
        public CategorySelection(ObservableCollection<Category> categories, Game selectedGame)
        {
            InitializeComponent();
            cateRepo = new CategoryRepository();
            _categories = categories;
            _selectedGame = selectedGame;
            CategoryListBox.ItemsSource = _categories;
            ActionButton.Content = "Add Game to Category";
            ActionButton.Click += AddGameToCategoryButton_Click;
        }

        public CategorySelection(ObservableCollection<Category> categories, Game selectedGame, Gamer gamer)
        {
            InitializeComponent();
            cateRepo = new CategoryRepository();
            gameRepository = new GameRepository();
            _selectedGame = selectedGame;
            // Async method should be awaited
            LoadCategoriesAsync(gamer, selectedGame);

            ActionButton.Content = "Remove game from Category";
            ActionButton.Click += RemoveGameFromCategoryButton_Click;
        }

        // Use async method to load categories
        private async void LoadCategoriesAsync(Gamer gamer, Game selectedGame)
        {
            // Await the result of GetCategoriesByGameAndGamerAsync
            var categories = await cateRepo.GetCategoriesByGameAndGamerAsync(gamer, selectedGame);

            // Convert to ObservableCollection after fetching data
            _categories = new ObservableCollection<Category>(categories);
            CategoryListBox.ItemsSource = _categories;
        }

        private async void RemoveGameFromCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = CategoryListBox.SelectedItem as Category;

            if (selectedCategory != null)
            {
                // Remove the selected game from the category's GameIds
                if (selectedCategory.GameIds.Contains(_selectedGame.GameID))
                {
                   await cateRepo.RemoveGameFromCategoryAsync(selectedCategory, _selectedGame);
                    MessageBox.Show("Game removed from the category successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("The game is not in the selected category.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a category to remove the game from.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        // Handler for deleting category
        private async void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = CategoryListBox.SelectedItem as Category;

            if (selectedCategory != null)
            {
               await cateRepo.DeleteCategoryAsync(selectedCategory);
              
                _categories.Remove(selectedCategory);

                MessageBox.Show("Category deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a category to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Handler for adding game to selected category
        private async void AddGameToCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = CategoryListBox.SelectedItem as Category;
        
            if (selectedCategory != null)
            {
                // Add the selected game to the category's GameIds
                if (!selectedCategory.GameIds.Contains(_selectedGame.GameID))
                {
                    await cateRepo.AddGameToCategoryAsync(selectedCategory, _selectedGame);
           

                    MessageBox.Show("Game added to the category successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                 
                    MessageBox.Show("The game is already in the selected category.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a category to add the game to.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

}

