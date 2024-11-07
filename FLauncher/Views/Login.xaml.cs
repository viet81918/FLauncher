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
        
        public Login()
        {
            InitializeComponent(); // Make sure this is called first
            _userRepo = new UserRepository();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }
        private void LoginGG_Click(object sender, RoutedEventArgs e)
        {
            var googleUser = GoogleLogin();
            //try
            //{
                
                if (googleUser != null)
                {
                    // Lấy người dùng từ cơ sở dữ liệu theo email Google
                    Model.User user = _userRepo.GetUserByEmail(googleUser.Email);

                    // Kiểm tra xem người dùng có tồn tại không
                    if (user != null)
                    {
                        
                            MessageBox.Show("Đăng nhập thành công!");
                            SaveUserInfoToJson(user);

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

        public GoogleJsonWebSignature.Payload GoogleLogin()
        {
            string clientId = "1031425762492-i8nm0g5v5sds03j1836u5rn01fm64d71.apps.googleusercontent.com";
            string redirectUri = "http://localhost:8080/"; // URI điều hướng sau khi xác thực
            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=email";
            string clientSecret = "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";

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = authUrl,
                UseShellExecute = true
            });

            // Đợi mã xác thực được trả về sau khi người dùng đăng nhập (giả sử bạn có một cách để nhận mã)
            string authorizationCode = LoginGoogle.GetAuthorizationCodeFromUser(); // Đây là phương thức bạn cần để nhận mã

            // Đổi mã xác thực lấy access token
            var tokenResponse = GetAccessToken(clientId, clientSecret, redirectUri, authorizationCode);

            // Sử dụng token để lấy thông tin người dùng
            var payload = GetGoogleUserInfo(tokenResponse.AccessToken);

            return payload;
        }
        // Yêu cầu HTTP POST tới Google, lấy access token
        public Model.TokenResponse GetAccessToken(string clientId, string clientSecret, string redirectUri, string authorizationCode)
        {
            var client = new HttpClient();
            var tokenRequest = new Dictionary<string, string>
         {
             { "code", authorizationCode },
             { "client_id", clientId },
             { "client_secret", clientSecret },
             { "redirect_uri", redirectUri },
             { "grant_type", "authorization_code" }
         };

            var content = new FormUrlEncodedContent(tokenRequest);
            var response = client.PostAsync("https://oauth2.googleapis.com/token", content).Result;
            var responseBody = response.Content.ReadAsStringAsync().Result;

            // Phân tích JSON để lấy access token
            var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.TokenResponse>(responseBody);
            return tokenResponse;
        }
        //Sử dụng access token để yêu cầu thông tin người dùng từ Google.
        public GoogleJsonWebSignature.Payload GetGoogleUserInfo(string accessToken)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = client.Send(request);
            var responseBody = response.Content.ReadAsStringAsync().Result;

            // Phân tích JSON để lấy thông tin người dùng
            var payload = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleJsonWebSignature.Payload>(responseBody);
            return payload;
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
                    else if (user.Role == 3 || user.Role == 2)
                    {
                        SaveUserInfoToJson(user);
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

        private void SaveUserInfoToJson(Model.User user)
        {
            try
            {
                // Convert user object to JSON format
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(user, Formatting.Indented);

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
            else if (accountType == "customer")
            {
                MessageBox.Show("Đăng nhập thành công với tư cách khách hàng!");
                Model.User loggedInUser = _userRepo.GetUserByEmailPass(enteredUserEmail, enteredPassword);
                CustomerWindow customerWindow = new CustomerWindow(loggedInUser);
                customerWindow.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.");               
            }
        }
    }
}
