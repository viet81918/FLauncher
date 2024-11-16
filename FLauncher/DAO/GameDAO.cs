using FLauncher.Model;
using FLauncher.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using SharpCompress.Archives;
using System.Windows;
using MongoDB.Bson;

namespace FLauncher.DAO
{
    public class GameDAO : SingletonBase<GameDAO>
    {

        private const string PathtoServiceAccountKeyfile = @"E:\FPT UNI\OJT\MOCK\FLauncher\FLauncher\credentials.json";
        private readonly FlauncherDbContext _dbContext;

        public GameDAO()
        {
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
        }

        public void DownloadRarFromFolder(Game game, string saveLocation, Gamer gamer)
        {
            // 1. Tạo credential
            var credential = GoogleCredential.FromFile(PathtoServiceAccountKeyfile)
                .CreateScoped(new[] { DriveService.ScopeConstants.DriveReadonly });

            // 2. Khởi tạo service
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            // 3. Lấy folder ID từ link
            string folderId = GetFolderIdFromLink(game.GameLink);
            MessageBox.Show($"Folder ID: {folderId}");

            // 4. Lấy file ID của file .rar duy nhất trong thư mục
            string rarFileId = GetRarFileIdFromFolder(service, folderId);
            if (string.IsNullOrEmpty(rarFileId))
            {
                MessageBox.Show("Không tìm thấy file RAR trong thư mục.");
                return;
            }

            // 5. Tải file .rar xuống
            string rarFilePath = Path.Combine(saveLocation, game.Name + ".rar");
            var request = service.Files.Get(rarFileId);

            // Đảm bảo thư mục lưu trữ tồn tại
            if (!Directory.Exists(saveLocation))
            {
                Directory.CreateDirectory(saveLocation);
                MessageBox.Show($"Thư mục lưu trữ không tồn tại, đã tạo thư mục: {saveLocation}");
            }

            MessageBox.Show("Bắt đầu tải file RAR...\nNhấn Ok để tắt thông báo, khi nào hoàn thành sẽ thông báo.");

            try
            {
                using (var fileStream = new FileStream(rarFilePath, FileMode.Create, FileAccess.Write))
                {
                    request.Download(fileStream);
                }
                // Tính kích thước file sau khi tải xong
                FileInfo fileInfo = new FileInfo(rarFilePath);
                double fileSizeInMB = Math.Round(fileInfo.Length / (1024.0 * 1024.0), 2); // Tính kích thước (MB), làm tròn đến 2 chữ số thập phân

                // Notify on successful download
                MessageBox.Show($"Tải file RAR hoàn tất: {rarFilePath}");

                // 6. Giải nén file RAR
                string extractFolderPath = Path.Combine(saveLocation, game.Name);
                if (!Directory.Exists(extractFolderPath))
                {
                    Directory.CreateDirectory(extractFolderPath);
                }

                ExtractRarFile(rarFilePath, extractFolderPath);

                // 7. Tạo object Download và lưu thông tin vào MongoDB
                var download = new Download
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    GamerId = gamer.GamerId,
                    GameId = game.GameID,
                    TimeDownload = DateTime.Now,
                    Storage = $"{fileSizeInMB} MB", // Lưu kích thước file RAR
                    Directory = extractFolderPath // Lưu vị trí thư mục đã giải nén
                };

                SaveDownloadToDatabase(download);
                MessageBox.Show("Lưu thông tin download vào cơ sở dữ liệu thành công.");
            }
            catch (Exception ex)
            {
                // Show error if download fails
                MessageBox.Show($"Lỗi khi tải file hoặc giải nén: {ex.Message}");
            }
        }
        private void SaveDownloadToDatabase(Download download)
        {
           _dbContext.Downloads.Add(download);  
            _dbContext.SaveChanges();
        }
        // Hàm lấy ID từ link
        private string ExtractRarFile(string rarFilePath, string extractFolderPath)
        {
            try
            {
                string extractedRootFolderPath = null;
                bool isRootFolderSet = false;

                // Sử dụng SharpCompress để giải nén file RAR
                using (var archive = ArchiveFactory.Open(rarFilePath))
                {
                    // Kiểm tra nếu file RAR chỉ chứa một thư mục duy nhất
                    var directories = archive.Entries
                                             .Where(entry => entry.IsDirectory)
                                             .Select(entry => entry.Key)
                                             .Distinct()
                                             .ToList();

                    // Nếu chỉ có một thư mục trong RAR, lấy tên thư mục đó
                    if (directories.Count == 1)
                    {
                        string rootFolderName = directories[0].TrimEnd('/');
                        extractedRootFolderPath = Path.Combine(extractFolderPath, rootFolderName);
                    }
                    else
                    {
                        // Nếu không có hoặc có nhiều thư mục, giải nén vào thư mục chung
                        extractedRootFolderPath = extractFolderPath;
                    }

                    // Giải nén tất cả các entry
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            // Giải nén vào thư mục
                            entry.WriteToDirectory(extractedRootFolderPath, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                }

                // Thông báo thành công
                MessageBox.Show($"Giải nén thành công tại: {extractedRootFolderPath}");

                // Xóa file RAR sau khi giải nén
                if (File.Exists(rarFilePath))
                {
                    File.Delete(rarFilePath);
                    MessageBox.Show($"Đã xóa file RAR: {rarFilePath}");
                }

                // Trả về đường dẫn thư mục đã giải nén
                return extractedRootFolderPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi giải nén file RAR: {ex.Message}");
                return null;
            }
        }
        private string GetFolderIdFromLink(string link)
        {
            var uri = new Uri(link);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            string folderId = query.Get("id");

            if (string.IsNullOrEmpty(folderId))
            {
                var segments = uri.Segments;
                if (segments.Length >= 4 && segments[1] == "drive/" && segments[2] == "folders/")
                {
                    folderId = segments[3].TrimEnd('/');
                }
            }

            return folderId;
        }

        // Hàm lấy file ID từ thư mục Google Drive
        private string GetRarFileIdFromFolder(DriveService service, string folderId)
        {
            var listRequest = service.Files.List();
            listRequest.Q = $"'{folderId}' in parents"; // Tìm tất cả các file trong thư mục
            listRequest.Fields = "files(id, name, mimeType)"; // Lấy thêm thông tin về mimeType
            listRequest.PageSize = 10;

            try
            {
                var files = listRequest.Execute().Files;

                if (files == null || files.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy file trong thư mục.");
                    return null; // Return null if no files found
                }

                // Hiển thị thông tin các file để kiểm tra
                foreach (var file in files)
                {
                    MessageBox.Show($"Tên file: {file.Name}, ID: {file.Id}, MIME Type: {file.MimeType}");
                }

                // Trả về ID của file đầu tiên
                return files[0].Id;
            }
            catch (Google.GoogleApiException ex)
            {
                MessageBox.Show($"Google API Error: {ex.Message}");
                if (ex.Error != null)
                {
                    MessageBox.Show($"Error code: {ex.Error.Code}");
                    MessageBox.Show($"Error message: {ex.Error.Message}");
                }
                throw; // Re-throw the exception for further handling
            }
        }

        public async Task<IEnumerable<Game>> GetTopGames()
        {
            return await _dbContext.Games
                .OrderByDescending(g => g.NumberOfBuyers) // Sắp xếp giảm dần theo NumberOfBuyers
                .Take(9) // Lấy ra 9 game đầu tiên
                .ToListAsync(); // Chuyển kết quả thành 
        }
            public  async Task<IEnumerable<Game>> GetGamesByGamer(Gamer gamer)
        {
            // Lấy danh sách các GameID mà người chơi đã mua từ bảng Bills
            var purchasedGameIds = _dbContext.Bills
                                              .Where(b => b.GamerId == gamer.Id)
                                              .Select(b => b.GameId)
                                              .ToList();

            // Lấy thông tin các game từ bảng Games dựa trên danh sách GameID đã mua
            var games = _dbContext.Games
                                  .Where(g => purchasedGameIds.Contains(g.GameID))
                                   .ToListAsync();
            return await games;
        }
      


    }
}
