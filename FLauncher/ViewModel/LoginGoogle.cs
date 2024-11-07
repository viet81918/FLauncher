using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FLauncher.ViewModel
{
    public class LoginGoogle
    {
        public static string GetAuthorizationCodeFromUser()
        {
            string authorizationCode = null;

            // Khởi tạo HttpListener để lắng nghe yêu cầu từ trình duyệt
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/"); // Địa chỉ localhost:8080 để lắng nghe

            try
            {
                listener.Start(); // Bắt đầu lắng nghe yêu cầu từ trình duyệt

                // Hiển thị thông báo yêu cầu người dùng nhập mã xác thực
                Console.WriteLine("Chờ người dùng đăng nhập Google...");

                // Chờ yêu cầu đến
                HttpListenerContext context = listener.GetContext();

                // Lấy mã xác thực từ URL
                var queryString = context.Request.Url.Query;
                var queryParams = HttpUtility.ParseQueryString(queryString);
                authorizationCode = queryParams["code"]; // Trích xuất mã xác thực từ query string

                // Trả lời lại cho trình duyệt để xác nhận hoàn tất
                string responseMessage = "<html><body>You can close this tab.</body></html>";
                byte[] buffer = Encoding.UTF8.GetBytes(responseMessage);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();

                Console.WriteLine("Mã xác thực nhận được: " + authorizationCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Có lỗi khi nhận mã xác thực: " + ex.Message);
            }
            finally
            {
                listener.Stop(); // Dừng listener khi hoàn tất               
            }

            return authorizationCode;
        }
    }
}
