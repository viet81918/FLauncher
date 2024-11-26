using FLauncher.Model;
using Microsoft.Win32;
using MongoDB.Bson;
using System.Windows;

namespace FLauncher.Views
{
    public partial class AddAchievementWindow : Window
    {
        public Achivement Achievement { get;  set; }

        private string _unlockIconPath;
        private string _lockIconPath;
        public bool _isEditMode;

        public AddAchievementWindow(Achivement achievement = null)
        {
            InitializeComponent();

            if (achievement != null)
            {
                _isEditMode = true;
                Achievement = achievement;
                Title = "Edit Achievement";
              
            }
            else
            {
                _isEditMode = false;
                Title = "Add Achievement";
            }
        }

     

        private void BrowseUnlockIcon_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _unlockIconPath = openFileDialog.FileName;
                MessageBox.Show($"Unlock icon selected: {_unlockIconPath}");
            }
        }

        private void BrowseLockIcon_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _lockIconPath = openFileDialog.FileName;
                MessageBox.Show($"Lock icon selected: {_lockIconPath}");
            }
        }

        private void SaveAchievement_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AchievementIdTextBox.Text) &&
                string.IsNullOrWhiteSpace(NameTextBox.Text) &&
                string.IsNullOrWhiteSpace(DescriptionTextBox.Text) &&
                string.IsNullOrWhiteSpace(TriggerTextBox.Text) &&
                string.IsNullOrEmpty(_unlockIconPath) &&
                string.IsNullOrEmpty(_lockIconPath))
            {
                MessageBox.Show("No fields to update. Please provide at least one value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_isEditMode)
            {
                // Update only the fields where new values are provided
                if (!string.IsNullOrWhiteSpace(AchievementIdTextBox.Text))
                {
                    Achievement.AchivementId = AchievementIdTextBox.Text;
                }

                if (!string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    Achievement.Name = NameTextBox.Text;
                }

                if (!string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                {
                    Achievement.Description = DescriptionTextBox.Text;
                }

                if (!string.IsNullOrWhiteSpace(TriggerTextBox.Text))
                {
                    Achievement.Trigger = TriggerTextBox.Text;
                }

                if (!string.IsNullOrEmpty(_unlockIconPath))
                {
                    Achievement.UnlockImageLink = _unlockIconPath;
                }

                if (!string.IsNullOrEmpty(_lockIconPath))
                {
                    Achievement.LockImageLink = _lockIconPath;
                }
            }
            else
            {
                // Create a new achievement
                Achievement = new Achivement
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    AchivementId = AchievementIdTextBox.Text,
                    Name = NameTextBox.Text,
                    Description = DescriptionTextBox.Text,
                    Trigger = TriggerTextBox.Text,
                    UnlockImageLink = _unlockIconPath,
                    LockImageLink = _lockIconPath
                };
            }

            DialogResult = true;
            Close();
        }

    }
}
