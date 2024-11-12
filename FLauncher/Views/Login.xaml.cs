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
using Microsoft.VisualBasic.ApplicationServices;
using FLauncher.Repositories;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Net;
using FLauncher.ViewModel;


namespace FLauncher.Views
{
    public partial class Login : Window
    {
        private readonly IUserRepository _userRepo;
        private readonly IGamerRepository _gamerRepo;
        public Login()
        {
            InitializeComponent(); // Make sure this is called first
            _userRepo = new UserRepository();
            _gamerRepo = new GamerRepository();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }
        private void LoginGG_Click(object sender, RoutedEventArgs e)
        {
            var googleUser = LoginGoogle.GoogleLogin();
            //try
            //{
                
                if (googleUser != null)
                {
                    // Lấy người dùng từ cơ sở dữ liệu theo email Google
                    Model.User user = _userRepo.GetUserByEmail(googleUser.Email);
                    Model.Gamer gamer = _gamerRepo.GetGamerByUser(user);
                // Kiểm tra xem người dùng có tồn tại không
                if (user != null)
                    {
                        
                            MessageBox.Show("Đăng nhập thành công!");
                            SaveUserInfoToJson(gamer);
                            CustomerWindow customerWindow = new CustomerWindow(user);
                            customerWindow.Show();
                            this.Close();
                        
                    }
                    else
                    {
                        MessageBox.Show("Email: " + googleUser.Email + "chưa được đăng ký!");
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Đăng nhập thất bại: " + ex.Message + "," + googleUser.Email);
            //}
        }

        //login nhap tay
        private string CheckLogin(string emailUser, string password)
        {
            try
            {
                Model.User user = _userRepo.GetUserByEmailPass(emailUser, password);
                

                if (user != null)
                {
                    // Check the user's role
                    if (user.Role == 1)
                    {
                        return "admin";
                    }
                    else if (user.Role == 3)
                    {
                        Model.Gamer gamer = _gamerRepo.GetGamerByUser(user);
                        SaveUserInfoToJson(gamer);
                        return "gamer";
                    }
                    else
                    {
                        return "publisher";
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

        private void SaveUserInfoToJson(Model.Gamer gamer)
        {
            try
            {
                // Convert user object to JSON format
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(gamer, Formatting.Indented);

                // Save JSON to file
                System.IO.File.WriteAllText("D:\\userInfo.json", json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu thông tin người dùng: " + ex.Message);
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

        private  void PerformLogin()
        { 
            string enteredUserEmail = emailU.email.Text.Trim();
            string enteredPassword = passU.passbox.Password.Trim();

            string  accountType = CheckLogin(enteredUserEmail, enteredPassword);

            // Check the result of CheckLogin
            if (accountType == "admin")
            {
                MessageBox.Show("Đăng nhập thành công với tư cách quản trị viên!");
                MainWindow adminWindow = new MainWindow();
                adminWindow.Show();

                // Close the Login window
                this.Close();
            }
            else if (accountType == "gamer")
            {
                MessageBox.Show("Đăng nhập thành công với tư cách gamer!");
                Model.User loggedInUser = _userRepo.GetUserByEmailPass(enteredUserEmail, enteredPassword);
                CustomerWindow customerWindow = new CustomerWindow(loggedInUser);
                customerWindow.Show();

                this.Close();
            }
            else if (accountType == "publisher")
            {
                MessageBox.Show("Đăng nhập thành công với tư cách nhà phát hành!");
                Model.User loggedInUser = _userRepo.GetUserByEmailPass(enteredUserEmail, enteredPassword);
                CustomerWindow customerWindow = new CustomerWindow(loggedInUser);
                customerWindow.Show();

                this.Close();
            }
            else
            {

                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.");
                
                 // Close the Login window
                Window parentWindow = Window.GetWindow(this);
                if (parentWindow != null)
                {
                    parentWindow.Close();
                }
 
            }
        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // Mở URL trong trình duyệt mặc định
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true; // Đảm bảo sự kiện không bị xử lý thêm
        }

    }
}
