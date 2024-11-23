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
        public ObservableCollection<Model.Message> Messages { get; set; }
        public string Name => Gamer?.Name ?? GamePublisher?.Name;
        public double Money => Gamer?.Money ?? GamePublisher?.Money ?? 0.0;


        public MessageWindowViewModel(Gamer gamer, List<Gamer> friends, List<Model.Message> messages)
        {
            Gamer = gamer;
            Friends = new ObservableCollection<Gamer>(friends);
            Messages = new ObservableCollection<Model.Message>(messages);
        }

        

        // Property changed event to notify UI
        public event PropertyChangedEventHandler PropertyChanged;

        // Helper method to raise the PropertyChanged event
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateMessages(List<Model.Message> newMessages)
        {
            Messages.Clear(); // Làm mới danh sách tin nhắn
            foreach (var message in newMessages)
            {
                Messages.Add(message);
            }

            OnPropertyChanged(nameof(Messages));
        }
    }
}
