using FLauncher.Model;
using FLauncher.Repositories;
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
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        private Gamer _gamer;
        private readonly FriendRepository _friendRepo;
        public MessageWindow(Gamer gamer)
        {
            InitializeComponent();
            _friendRepo = new FriendRepository();
        }
    }
}
