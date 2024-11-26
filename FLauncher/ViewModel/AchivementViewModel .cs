using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.ViewModel
{
    public class AchivementViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Achivement> _achievements;
        public ObservableCollection<Achivement> Achievements
        {
            get => _achievements;
            set
            {
                _achievements = value;
                OnPropertyChanged(nameof(Achievements));
            }
        }

        private Achivement _selectedAchievement;
        public Achivement SelectedAchievement
        {
            get => _selectedAchievement;
            set
            {
                _selectedAchievement = value;
                OnPropertyChanged(nameof(SelectedAchievement));
            }
        }

        public AchivementViewModel(IEnumerable<Achivement> achievements)
        {
            Achievements = new ObservableCollection<Achivement>(achievements);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
