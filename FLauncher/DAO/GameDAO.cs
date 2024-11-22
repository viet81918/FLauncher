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
using System.Diagnostics;
using Google.Apis.Upload;
using Microsoft.IdentityModel.Tokens;

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


        private string GetFileIdFromLink(string link)
        {
            try
            {
                var uri = new Uri(link);

                // Log for debugging
                Console.WriteLine($"URL: {link}");
                Console.WriteLine($"Path: {uri.AbsolutePath}");

                // Ensure the path contains "/d/" (which is part of the Google Drive link)
                if (uri.AbsolutePath.Contains("/d/"))
                {
                    // Extract the part of the URL after "/d/" and before the next "/"
                    var fileIdStart = uri.AbsolutePath.IndexOf("/d/") + 3;  // Skip "/d/"
                    var fileIdEnd = uri.AbsolutePath.IndexOf("/", fileIdStart);

                    // If no slash found, take till the end of the string
                    if (fileIdEnd == -1)
                    {
                        fileIdEnd = uri.AbsolutePath.Length;
                    }

                    var fileId = uri.AbsolutePath.Substring(fileIdStart, fileIdEnd - fileIdStart);

                    // Log the extracted file ID
                    Console.WriteLine($"Extracted File ID: {fileId}");
                    return fileId;
                }

                // If the pattern is not found, log the failure
                Console.WriteLine("Link does not match expected pattern.");
                return null;
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur during processing
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }



        public void DownloadRarFromLink(Game game, string saveLocation, Gamer gamer)
        {
            try
            {
                // 1. Tạo credential
                var credential = GoogleCredential.FromFile(PathtoServiceAccountKeyfile)
                    .CreateScoped(new[] { DriveService.ScopeConstants.DriveReadonly });

                // 2. Khởi tạo service
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential
                });

                // 3. Lấy file ID từ link (for direct file download)
                string fileId = GetFileIdFromLink(game.GameLink);
                if (string.IsNullOrEmpty(fileId))
                {
                    MessageBox.Show("Không tìm thấy File ID từ link của game.");
                    return;
                }
                MessageBox.Show($"File ID: {fileId}"); // Debug: Log the file ID

                // 4. Tải file .rar xuống
                string rarFilePath = Path.Combine(saveLocation, game.Name + ".rar");
                var request = service.Files.Get(fileId);

                // Đảm bảo thư mục lưu trữ tồn tại
                if (!Directory.Exists(saveLocation))
                {
                    Directory.CreateDirectory(saveLocation);
                    MessageBox.Show($"Thư mục lưu trữ không tồn tại, đã tạo thư mục: {saveLocation}");
                }

                MessageBox.Show("Bắt đầu tải file RAR...\nNhấn Ok để tắt thông báo, khi nào hoàn thành sẽ thông báo.");

                // 5. Tải file xuống
                using (var fileStream = new FileStream(rarFilePath, FileMode.Create, FileAccess.Write))
                {
                    request.Download(fileStream);
                }

                // Tính kích thước file sau khi tải xong
                FileInfo fileInfo = new FileInfo(rarFilePath);
                double fileSizeInMB = Math.Round(fileInfo.Length / (1024.0 * 1024.0), 2); // Tính kích thước (MB)

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
        private bool DoesFileExist(DriveService service, string fileId)
        {
            try
            {
                var request = service.Files.Get(fileId);
                request.Fields = "id";
                var file = request.Execute();
                return file != null;
            }
            catch (Exception ex)
            {
                // If an exception is thrown, file might not exist
                MessageBox.Show($"Lỗi khi kiểm tra file: {ex.Message}");
                return false;
            }
        }

        private void SaveDownloadToDatabase(Download download)
        {
           _dbContext.Downloads.Add(download);  
            _dbContext.SaveChanges();
        }
   

        public async Task<IEnumerable<Game>> GetTopGames()
        {
            return await _dbContext.Games
                .OrderByDescending(g => g.NumberOfBuyers) // Sắp xếp giảm dần theo NumberOfBuyers
                .Take(9) // Lấy ra 9 game đầu tiên
                .ToListAsync(); // Chuyển kết quả thành 
        }

        public async Task<IEnumerable<Game>> GetAllGame()
        {
            return await _dbContext.Games
                .OrderByDescending(g => g.NumberOfBuyers) // Sắp xếp giảm dần theo NumberOfBuyers
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Game>> GetGameByInformation(string inputName, List<string> genres, string pubs)
        {
            // lay id game co ten bat dau bang input name ex: gta => gta 1 , 2 ,3 ...
            var GameN = new List<string>();
            if (!string.IsNullOrEmpty(inputName))
            {
                MessageBox.Show("inputName Game DAO la " + inputName);
                GameN = await _dbContext.Games
                    .Where(g => g.Name.ToLower().StartsWith(inputName))
                    .Select(n => n.GameID).ToListAsync();

                MessageBox.Show("so luong id trong GameN: " + GameN.Count);

                string NamdautieneG = string.Join(", ", GameN);
                MessageBox.Show($"danh sach id bame trong GameN: {NamdautieneG}");
            }

            

            //lay list id game co tat ca the loai trong List<string> genres truyen vao
           
            var GameGenre = new List<string>();
            if (!genres.IsNullOrEmpty())
            {
                var listGameHas = await _dbContext.GameHasGenres.ToListAsync();
                GameGenre = listGameHas
                    .GroupBy(e => e.GameId)
                    .Where(group => genres.All(genre => group.Any(g => g.TypeOfGenres == genre))) // Kiểm tra đủ thể loại
                    .Select(group => group.Key) // Lấy ID_Game
                    .ToList();

                MessageBox.Show("so luong id trong GameGenre: " + GameGenre.Count);
                string aftersacrh = string.Join(", ", GameGenre);
                MessageBox.Show($"danh sach GameGenre: {aftersacrh}");
            }

            

            //lay danh sach gid ame co publisher = pub truyen vao 
            MessageBox.Show("publisher la " + pubs);
            var pubInStore = await _dbContext.GamePublishers //lay dc id publisher 
                .Where(p => p.Name.Equals(pubs))
                .Select(b => b.PublisherId).ToListAsync();

            var GameP = await _dbContext.Publishcations // lay id game tu Publishcations bang id pub 
                .Where(s => pubInStore.Contains(s.GamePublisherId))
                .Select(c => c.GameId).ToListAsync();

            MessageBox.Show("So luong game id trong GameP: " + GameP.Count);

            //var GamesValid = GameN.Intersect(GameGenre).Intersect(GameP);

            // Khởi tạo GamesValid với danh sách không rỗng đầu tiên
            //IEnumerable<string> GamesValid = null;
            var GamesValid = new List<string>();
            // Nếu GameN không rỗng, gán nó vào GamesValid
            if (GameN.Any())
            {
                GamesValid = GameN;
            }
            // Nếu GameN rỗng nhưng GameGenre không rỗng, gán GameGenre vào GamesValid
            else if (GameGenre.Any())
            {
                GamesValid = GameGenre;
            }
            // Nếu cả GameN và GameGenre đều rỗng nhưng GameP không rỗng, gán GameP vào GamesValid
            else if (GameP.Any())
            {
                GamesValid = GameP;
            }
            // Nếu tất cả các danh sách đều rỗng, gán GamesValid là danh sách rỗng
            //else
            //{
            //    GamesValid = Enumerable.Empty<string>();
            //}

            // Nếu GameGenre không rỗng, thực hiện giao giữa GamesValid và GameGenre
            if (GameGenre.Any())
            {
                GamesValid = GamesValid.Intersect(GameGenre).ToList();
            }

            // Nếu GameP không rỗng, thực hiện giao giữa GamesValid và GameP
            if (GameP.Any())
            {
                GamesValid = GamesValid.Intersect(GameP).ToList();
            }
            MessageBox.Show("So luong game id trong GamesValid = " + GamesValid.Count());

            var resultG = await _dbContext.Games.Where(f => GamesValid.Contains(f.GameID)).ToListAsync();


            return resultG;
        }

        public async Task<IEnumerable<Game>> GetGamesByGamer(Gamer gamer)
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

        public void PlayGame(Game game, Gamer gamer)
        {
            try
            {
                // Retrieve the download directory from the database
                string downloadDirectory = _dbContext.Downloads
                    .Where(c => c.GameId == game.GameID && c.GamerId == gamer.GamerId)
                    .Select(b => b.Directory)
                    .FirstOrDefault();

                // Check if the directory was found
                if (string.IsNullOrWhiteSpace(downloadDirectory) || !Directory.Exists(downloadDirectory))
                {
                    Console.WriteLine("Download directory not found.");
                    return;
                }

                // Find the executable file containing the word "game" in its name
                string exeFilePath = Directory
                    .EnumerateFiles(downloadDirectory, "*.exe", SearchOption.AllDirectories)
                    .FirstOrDefault(file => Path.GetFileName(file).Contains("game", StringComparison.OrdinalIgnoreCase));

                // Check if an executable file was found
                if (exeFilePath == null)
                {
                    Console.WriteLine($"No executable file with 'game' in the name found in directory '{downloadDirectory}'.");
                    return;
                }

                // Launch the executable
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exeFilePath,
                    WorkingDirectory = Path.GetDirectoryName(exeFilePath),
                    UseShellExecute = true
                };

                Process.Start(startInfo);
                Console.WriteLine($"Game launched successfully: {exeFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error launching the game: {ex.Message}");
            }
        }

        public void Update_Game(GamePublisher publisher, Game game, string selectedFilePath, string message)
        {
            try
            {
                // Step 1: Authenticate with Google Drive API
                var credential = GoogleCredential.FromFile(PathtoServiceAccountKeyfile)
                    .CreateScoped(DriveService.ScopeConstants.Drive)
                    .UnderlyingCredential as ServiceAccountCredential;

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential
                });

                // Step 2: Get the file ID from the game link
                string fileId = GetFileIdFromLink(game.GameLink);
                if (string.IsNullOrEmpty(fileId))
                {
                    MessageBox.Show("Không tìm thấy File ID từ link của game.");
                    return;
                }

                // Step 3: Delete the existing file in Google Drive (if needed)
                DeleteFile(service, fileId);

                // Step 4: Upload the new file
                string uploadedFileId = UploadFile(service, selectedFilePath, Path.GetFileName(selectedFilePath));

                // Step 5: If the upload was successful, update the link in the database
                if (!string.IsNullOrEmpty(uploadedFileId))
                {
                    string shareableLink = GetShareableLink(service, uploadedFileId);
                    UpdateInfor(game, publisher, message);
                    UpdateLink(game, shareableLink);
                }

                MessageBox.Show("Cập nhật game thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật game: {ex.Message}");
            }
        }

        private void DeleteFile(DriveService service, string fileId)
        {
            try
            {
                service.Files.Delete(fileId).Execute();

                MessageBox.Show("Đã xóa file cũ khỏi Google Drive.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa file cũ: {ex.Message}");
            }
            if (DoesFileExist(service, fileId))
            {
                DeleteFile(service, fileId);
            }
            else
            {
                MessageBox.Show("File không tồn tại trong Google Drive.");
            }

        }


        public void UpdateInfor(Game game, GamePublisher gamePublisher, string message)
        {
            try
            {
                
                var update = new Update
                {

                    Id = ObjectId.GenerateNewId().ToString(),
                    PublisherId = gamePublisher.PublisherId,  // Assuming gamePublisher has an Id property
                    GameId = game.GameID,  // Assuming game has an Id property
                    UpdateContent = message,  // The message or content of the update
                    UpdateTimeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")  // Set the update time
                };
                _dbContext.Updates.Add(update);
                _dbContext.SaveChanges();
              
                // Step 3: Inform the user
                MessageBox.Show("Đã cập nhật game thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật game: {ex.Message}");
            }
        }
        private void UpdateLink(Game game, string link)
        {
            game.GameLink = link;
            _dbContext.Games.Update(game);  // Cập nhật game trong database
            _dbContext.SaveChanges();
            MessageBox.Show("Đã cập nhật link game thành công!");
        }
        private string UploadFile(DriveService service, string filePath, string fileName)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
            };

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var uploadRequest = service.Files.Create(fileMetadata, fileStream, "application/octet-stream");
                uploadRequest.Fields = "id, name";
                var progress = uploadRequest.Upload();

                if (progress.Status == UploadStatus.Completed)
                {
                    var uploadedFile = uploadRequest.ResponseBody;
                    MessageBox.Show($"Tải file mới lên thành công: {uploadedFile.Name}");

                    // Return file ID for further operations
                    return uploadedFile.Id;
                }
                else
                {
                    MessageBox.Show($"Lỗi khi tải file lên: {progress.Exception.Message}");
                    return null;
                }
            }
        }

        private string GetShareableLink(DriveService service, string fileId)
        {
            try
            {
                // Set file permission to "Anyone with the link" and "Reader" (view-only access)
                var permission = new Google.Apis.Drive.v3.Data.Permission()
                {
                    Type = "anyone", // Allows anyone with the link to access
                    Role = "writer"  // Valid role is "reader" for view-only access
                };

                // Create shareable permission
                service.Permissions.Create(permission, fileId).Execute();

                // Retrieve the shareable link
                var fileRequest = service.Files.Get(fileId);
                fileRequest.Fields = "webViewLink";  // Get the view link
                var fileInfo = fileRequest.Execute();

                return fileInfo.WebViewLink;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thiết lập quyền chia sẻ: {ex.Message}");
                return null;
            }
        }



    }
}
