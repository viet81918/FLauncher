using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
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

        public static GoogleJsonWebSignature.Payload GoogleLogin()
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
        public static Model.TokenResponse GetAccessToken(string clientId, string clientSecret, string redirectUri, string authorizationCode)
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
        public static GoogleJsonWebSignature.Payload GetGoogleUserInfo(string accessToken)
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
    }
}
