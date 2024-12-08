using FLauncher.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace FLauncher.Views
{
    public partial class SelectCategoryWindow : Window
    {
        public Category SelectedCategory { get; private set; }

        // Constructor that takes a list of categories and displays them
        public SelectCategoryWindow(ObservableCollection<Category> categories)
        {
            InitializeComponent();
            // Directly set DataContext to the ObservableCollection of categories
            this.DataContext = categories;
        }

        // Handler for the "Select" button
        private void CategoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update the SelectedCategory when a category is selected in the CategoriesListBox
            SelectedCategory = CategoriesListBox.SelectedItem as Category;
        }

        // Handles the click on the Select button
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCategory != null && !string.IsNullOrWhiteSpace(SelectedCategory.NameCategories))
            {
                // If a valid category is selected, set the SelectedCategory and close the window
                DialogResult = true;
                Close();
            }
            else
            {
                // If no category is selected or the name is empty, show an error
                MessageBox.Show("Please select a category.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Handler for the "Cancel" button
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window without selection
            DialogResult = false;
            Close();
        }
    }
}
