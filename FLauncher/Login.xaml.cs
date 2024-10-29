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


namespace FLauncher
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();

        }
        private string CheckLogin(string emailUser, string password)
        {

            try
            {
                //// Đọc file JSON từ hệ thống
                //string jsonFilePath = "appsettings.json";
                //string jsonData = File.ReadAllText(jsonFilePath);

                //// Chuyển đổi JSON thành danh sách đối tượng User
                //List<AccountUser> users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AccountUser>>(jsonData);

                //return (users.Any(user => user.Email == emailUser && user.Password == password));

                //1.connect data 
                var dbClient = new MongoClient("mongodb://localhost:27017");
                IMongoDatabase db = dbClient.GetDatabase("Sony");
                var emp = db.GetCollection<BsonDocument>("Employees");
                //2. truy van 
                //Filter.Regex tìm các giá trị  khớp với một mẫu 
                //Filter.Eq tìm các giá trị bằng chính xác với giá trị được cung cấp.
                var fil = /*Builders<BsonDocument>.Filter.Eq("ID", new BsonRegularExpression("^admin_")) &*/
                            Builders<BsonDocument>.Filter.Eq("Email", emailUser) &
                             Builders<BsonDocument>.Filter.Eq("Password", password); 
                             

                var doc = emp.Find(fil).FirstOrDefault();


                if (doc != null)
                {
                    var userid = doc["ID"].ToString();
                    if (userid.StartsWith("admin"))
                    {
                        return "admin"; // Trả về loại tài khoản admin
                    }
                    else
                    {
                        return "customer"; // Trả về loại tài khoản customer
                    }

                }
                MessageBox.Show("Không tìm thấy người dùng.");
                return null;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đọc file JSON: " + ex.Message);
                return null;
            }
        }
        // enter button event  
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
            string enteredUserEmail = Convert.ToString(emailU.email.Text.Trim());

            string enteredPassword = Convert.ToString(passU.passbox.Password.Trim());

            string accountType = CheckLogin(enteredUserEmail, enteredPassword);
            // Kiểm tra đăng nhập
            if (accountType == "admin")
            {
                MessageBox.Show("Đăng nhập thành công!");
                // Chuyển đến màn hình tiếp theo hoặc thực hiện các hành động cần thiết
                
                MainWindow adminWindow = new MainWindow();
                adminWindow.Show();

                
            }
            else if(accountType == "customer")
            {
                MessageBox.Show("Đăng nhập thành công!");
                // Chuyển đến màn hình tiếp theo hoặc thực hiện các hành động cần thiết

                CustomerWindow customerWindow = new CustomerWindow();
                customerWindow.Show();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.");
            }
            //đóng window chứa LoginPage
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }
    }
}
