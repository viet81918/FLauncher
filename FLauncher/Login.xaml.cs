using FLauncher.Model;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.IO;
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
using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Driver;
using static System.Net.WebRequestMethods;
using FLauncher.Services;


namespace FLauncher
{
    public partial class Login : Window
    {
        private readonly IUserService _userService;

        public Login()
        {
            InitializeComponent(); // Make sure this is called first
            _userService = new UserService();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }

        private async Task<string> CheckLogin(string emailUser, string password)
        {
            try
            {
                User user = await _userService.GetUserByEmailPass(emailUser, password);

                if (user != null)
                {
                    // Check the user's role
                    if (user.Role == 1)
                    {
                        return "admin";
                    }
                    else if (user.Role == 3)
                    {
                        return "customer";
                    }
                }

                MessageBox.Show("Không tìm thấy người dùng.");
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kết nối với cơ sở dữ liệu: " + ex.Message);
                return null;
            }
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformLogin();
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformLogin();
            }
        }

        private async void PerformLogin()
        {
            string enteredUserEmail = emailU.email.Text.Trim();
            string enteredPassword = passU.passbox.Password.Trim();

            string  accountType = await CheckLogin(enteredUserEmail, enteredPassword);

            // Check the result of CheckLogin
            if (accountType == "admin")
            {
                MessageBox.Show("Đăng nhập thành công với tư cách quản trị viên!");
                MainWindow adminWindow = new MainWindow();
                adminWindow.Show();
            }
            else if (accountType == "customer")
            {
                MessageBox.Show("Đăng nhập thành công với tư cách khách hàng!");
                CustomerWindow customerWindow = new CustomerWindow();
                customerWindow.Show();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            // Close the Login window
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }
    }

}
