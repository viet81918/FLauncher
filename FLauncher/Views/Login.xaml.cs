using FLauncher.Repositories;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Input;


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

        }

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

        private void PerformLogin()
        {
            string enteredUserEmail = emailU.email.Text.Trim();
            string enteredPassword = passU.passbox.Password.Trim();

            string accountType = CheckLogin(enteredUserEmail, enteredPassword);


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
                /*
                 // Close the Login window
                Window parentWindow = Window.GetWindow(this);
                if (parentWindow != null)
                {
                    parentWindow.Close();
                }
                 */

            }
        }
    }
}
