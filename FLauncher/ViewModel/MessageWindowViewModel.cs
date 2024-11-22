using FLauncher.Model;
using FLauncher.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace FLauncher.ViewModel
{
    public class MessageWindowViewModel : INotifyPropertyChanged
    {
        public Gamer Gamer { get; }
        public GamePublisher GamePublisher { get; }
        public ObservableCollection<Gamer> Friends { get; }
        private ObservableCollection<Model.Message> _Messages { get; set; }
        public ObservableCollection<Model.Message> Messages
        {
            get => _Messages;
            set
            {
                if (_Messages != value)
                {
                    _Messages = value;
                    OnPropertyChanged(nameof(Messages));
                }
            }
        }
        public string Name => Gamer?.Name ?? GamePublisher?.Name;
        public double Money => Gamer?.Money ?? GamePublisher?.Money ?? 0.0;


        public MessageWindowViewModel(Gamer gamer, List<Gamer> friends, List<Model.Message> messages)
        {
            Gamer = gamer;
            Friends = new ObservableCollection<Gamer>(friends);
            Messages = new ObservableCollection<Model.Message>(messages);
        }

        //chat
        //public void LoadMessages(List<Model.Message> messages)
        //{
        //    Messages.Clear();
        //    foreach (var message in messages)
        //    {
        //        Messages.Add(message);
        //    }
        //}

        // Property changed event to notify UI
        public event PropertyChangedEventHandler PropertyChanged;

        // Helper method to raise the PropertyChanged event
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
