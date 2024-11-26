﻿using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using FLauncher.ViewModel;
using FLauncher.DAO;
using MongoDB.Bson;
using InternalVisibleTo;
using System.Windows.Threading;


namespace FLauncher.Views
{  
    public partial class MessageWindow : Window
    {
        private  Gamer _gamer;
        private User _user;
        private Friend _friend;
        private List<Model.Message> Messages;
        private readonly GamerRepository _gamerRepo;
        private readonly FriendRepository _friendRepo;
        private readonly FriendService _friendService;
        private readonly UserRepository _userRepo;
        private readonly MessageRepository _messageRepo;
        private Gamer _selectedFriend;

        private DispatcherTimer _messageUpdateTimer;
        public MessageWindow(Gamer gamer)
        {
            InitializeComponent();
            _userRepo = new UserRepository();
            _friendRepo = new FriendRepository();
            _messageRepo = new MessageRepository();
            _user = _userRepo.GetUserByGamer(gamer);
            _gamer = gamer;
            var listFriend = _friendRepo.GetAllFriendByGamer(gamer);
            Messages = new List<Model.Message>();
            DataContext = new MessageWindowViewModel(gamer, listFriend, Messages);

            lvFriends.ItemsSource = listFriend;

            // Khởi tạo và cấu hình Timer
            _messageUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2) // Cập nhật mỗi 2 giây
            };
            _messageUpdateTimer.Tick += MessageUpdateTimer_Tick;
        }

        private void lvFriends_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvFriends.SelectedItem is Gamer selectedFriend)
            {
                _selectedFriend = selectedFriend;
                // Load tin nhắn với bạn bè được chọn
                var viewModel = DataContext as MessageWindowViewModel;
                if (viewModel != null)
                {
                    viewModel.SelectedFriend.Clear();
                    viewModel.SelectedFriend.Add(selectedFriend); // Thêm người bạn đã chọn
                }

                Messages = _messageRepo.GetMessages(_gamer.GamerId, _selectedFriend.GamerId);
                ChatMessages.ItemsSource = Messages;
                ScrollToBottom();
                _messageUpdateTimer.Start();
            }
        }
        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            var messageContent = txtMessage.Text.Trim();
            if (!string.IsNullOrEmpty(messageContent))
            {
                var selectedFriend = lvFriends.SelectedItem as Gamer;
                if (selectedFriend != null)
                {
                    var message = new Model.Message
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        IdSender = _gamer.GamerId,
                        IdReceiver = selectedFriend.GamerId,
                        Content = messageContent,
                        TimeString = DateTime.Now
                    };
                    _messageRepo.SendMessage(message);
                    Messages.Add(message);
                    Messages = _messageRepo.GetMessages(_gamer.GamerId, selectedFriend.GamerId);
                    //ChatMessages.ItemsSource = null; // Đặt lại ItemsSource để cập nhật UI
                    ChatMessages.ItemsSource = Messages;

                    /*
                     
                    ChatMessages.ItemsSource = Messages;
                     */
                    ScrollToBottom();
                    // Xóa nội dung textbox
                    txtMessage.Clear();
                }
            }
        }
        private  void MessageUpdateTimer_Tick(object? sender, EventArgs e)
        {
            var selectedFriendU = lvFriends.SelectedItem as Gamer;
            if (selectedFriendU != null)
            {
                // Lấy tin nhắn mới nhất
                var updatedMessages = _messageRepo.GetMessages(_gamer.GamerId, selectedFriendU.GamerId);

                if (updatedMessages.Count != Messages.Count) // So sánh số lượng tin nhắn
                {
                    Messages = updatedMessages;
                    ChatMessages.ItemsSource = null;
                    ChatMessages.ItemsSource = Messages;
                    ScrollToBottom();
                }
            }
        }
        private void message_keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage_Click(sender, e);
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
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            //Close the App
            Close();
        }
        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            //First detect if windows is in normal state or maximized
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void Message_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtMessage.Text == "Type your message")
            {
                txtMessage.Text = string.Empty;
                txtMessage.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
        private void Message_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                txtMessage.Text = "Type your message";
            }
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {               
            if (SearchTextBox.Text == "Search name game")
            {
                SearchTextBox.Text = string.Empty;
                SearchTextBox.Foreground = (System.Windows.Media.Brush)Application.Current.Resources["SecondaryBrush"];
            }
        }       
        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Search name game";
            }
        }
        
        private void messageButton_Click(Object sender, MouseButtonEventArgs e)
        {
            var currentGamer = _gamer;
            var messWindow = new MessageWindow(currentGamer);
            messWindow.Show();
            this.Hide();
            this.Close();
        }
        private void logoutButton_Click(object sender, MouseButtonEventArgs e)
        {
            var result = MessageBox.Show("Bạn muốn đăng xuất?", "Xác nhận đăng xuất", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DeleteLoginInfoFile();
                this.Hide();
                Login login = new Login();
                login.Show();

                this.Close();
            }
        }
        private void DeleteLoginInfoFile()
        {
            string appDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FLauncher");
            string jsonFilePath = System.IO.Path.Combine(appDataPath, "loginInfo.json");

            if (File.Exists(jsonFilePath))
            {
                File.Delete(jsonFilePath);
            }
        }
        private void ProfileIcon_Click(object sender, MouseButtonEventArgs e)
        {
            // Create an instance of ProfileWindow and show it
            ProfileWindow profileWindow = new ProfileWindow(_user, _friendService);
            profileWindow.Show();
            this.Hide();
            this.Close();
            // Optionally, close the current window (MainWindow)
            // this.Close();
        }
        private void Home_Click(object sender, MouseButtonEventArgs e)
        {
            CustomerWindow cus = new CustomerWindow(_user);
            cus.Show();
            this.Hide();
            this.Close();
        }
        private void MyGame_Click(object sender, RoutedEventArgs e)
        {

            MyGame myGameWindow = new MyGame(_user);
            myGameWindow.Show();
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
            var CurrentWin = _user;
            string Search_input = SearchTextBox.Text.Trim().ToLower();
            if (Search_input == "search name game")
            {
                Search_input = string.Empty;
            }
            SearchWindow search = new SearchWindow(CurrentWin, Search_input, null, null);
            search.Show();
            this.Hide();
            this.Close();
        }
        private void searchButton_Click(object sendedr, MouseButtonEventArgs e)
        {
            var CurrentUser = _user;
            SearchWindow serchwindow = new SearchWindow(CurrentUser, null, null, null);
            serchwindow.Show();
            this.Hide();
            this.Close();
        }

        private void ScrollToBottom()
        {
            ChatScrollViewer.ScrollToEnd();
        }

    }
}
