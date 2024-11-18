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
            this.Loaded += Window_Loaded;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string appDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FLauncher");
            string jsonFilePath = System.IO.Path.Combine(appDataPath, "loginInfo.json");

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            if (System.IO.File.Exists(jsonFilePath))
            {
                var json = System.IO.File.ReadAllText(jsonFilePath);
                var loginInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.AutoLogin>(json);
                //var ExpirationDate = DateTime.Now.AddMonths(1);
                if (loginInfo.ExpirationDate >= DateTime.Now)
                {
                    emailU.email.Text = loginInfo.Email;
                    passU.passbox.Password = loginInfo.Password;
                    // Thực hiện đăng nhập tự động
                    PerformLogin(loginInfo.Email, loginInfo.Password);
                }
                else
                {
                    System.IO.File.Delete(jsonFilePath); // Xóa tệp nếu hết hạn
                }
            }
            else
            {
                MessageBox.Show("khong tim thay file loginInfo.json");
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string enteredUserEmail = emailU.email.Text.Trim();
            string enteredPassword = passU.passbox.Password.Trim();
            if (RememberCheckBox.IsChecked == true)
            {
                var loginInfo = new Model.AutoLogin
                {
                    Email = emailU.email.Text.Trim(),
                    Password = passU.passbox.Password.Trim(), // giả sử passU là PasswordBox
                    ExpirationDate = DateTime.Now.AddMonths(1)
                };

                string appDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FLauncher");
                string jsonFilePath = System.IO.Path.Combine(appDataPath, "loginInfo.json");

                if (!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                }

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(loginInfo, Formatting.Indented);
                System.IO.File.WriteAllText(jsonFilePath, json);
                MessageBox.Show("da luu");
            }
            PerformLogin(enteredUserEmail, enteredPassword);
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
                passU.passbox.Focus(); ;
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender,e);
            }
        }

        private  void PerformLogin(string UserEmail, string UserPassword)
        {
            //string enteredUserEmail = emailU.email.Text.Trim();
            //string enteredPassword = passU.passbox.Password.Trim();

            string  accountType = CheckLogin(UserEmail, UserPassword);

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
                Model.User loggedInUser = _userRepo.GetUserByEmailPass(UserEmail, UserPassword);
                CustomerWindow customerWindow = new CustomerWindow(loggedInUser);
                customerWindow.Show();

                this.Close();
            }
            else if (accountType == "publisher")
            {
                MessageBox.Show("Đăng nhập thành công với tư cách nhà phát hành!");
                Model.User loggedInUser = _userRepo.GetUserByEmailPass(UserEmail, UserPassword);
                CustomerWindow customerWindow = new CustomerWindow(loggedInUser);
                customerWindow.Show();

                this.Close();
            }
            else
            {

                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.");
                
                 // Close the Login window
                //Window parentWindow = Window.GetWindow(this);
                //if (parentWindow != null)
                //{
                //    parentWindow.Close();
                //}
 
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
