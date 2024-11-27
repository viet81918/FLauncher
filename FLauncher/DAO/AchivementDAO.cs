using FLauncher.Model;
using FLauncher.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FLauncher.DAO
{
    public class AchivementDAO : SingletonBase<AchivementDAO>
    {
        private const string PathtoServiceAccountKeyfile = @"E:\FPT UNI\OJT\MOCK\FLauncher\FLauncher\credentials.json";
        private readonly FLauncherOnLineDbContext _dbContext;
        public AchivementDAO()
        {

            var connectionString = "mongodb+srv://viet81918:conchode239@cluster0.hzr2fsy.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
            var client = new MongoClient(connectionString);
            _dbContext = FLauncherOnLineDbContext.Create(client.GetDatabase("FPT"));
        }

        public async Task<IEnumerable<Achivement>> GetAchivementFromGame(Game game)
        {

            return await _dbContext.Achivements.Where(c => c.GameId == game.GameID).ToListAsync();
        }
        public async Task<IEnumerable<UnlockAchivement>> GetUnlockAchivements(IEnumerable<Achivement> achivements, Gamer gamer)
        {
            // Get the AchievementId and GameId from the list of achievements
            var achievementIds = achivements.Select(a => a.AchivementId).Distinct();
            var gameIds = achivements.Select(a => a.GameId).Distinct();


            // Query the UnlockAchivements based on multiple AchievementId and GameId
            return await _dbContext.UnlockAchivements
                .Where(c => c.GamerId == gamer.GamerId &&
                            achievementIds.Contains(c.AchievementId) &&
                            gameIds.Contains(c.GameId))
                .ToListAsync();

           
           
        }
        public async Task<IEnumerable<Achivement>> GetLockAchivement(IEnumerable<Achivement> achivements, Gamer gamer)
        {
            // Lấy danh sách AchievementId từ danh sách achievements
            var achievementIds = achivements.Select(a => a.AchivementId).Distinct();
            var gameIds = achivements.Select(a => a.GameId).Distinct();

            // Lấy danh sách các UnlockAchivement đã được gamer unlock
            var unlockedAchievementIds = await _dbContext.UnlockAchivements
                .Where(ua => ua.GamerId == gamer.GamerId &&
                             achievementIds.Contains(ua.AchievementId) &&
                             gameIds.Contains(ua.GameId))
                .Select(ua => ua.AchievementId)
                .Distinct()
                .ToListAsync();

            // Lấy danh sách các Achivement chưa được unlock (lock achievement)
            var lockedAchievements = await _dbContext.Achivements
                .Where(a => achievementIds.Contains(a.AchivementId) &&
                            gameIds.Contains(a.GameId) &&
                            !unlockedAchievementIds.Contains(a.AchivementId))
                .ToListAsync();

            return lockedAchievements;
        }

        public async Task<IEnumerable<Achivement>> GetAchivementsFromUnlocks(IEnumerable<UnlockAchivement> unlockAchivements)
        {
            // Lấy danh sách AchievementId và GameId từ unlockAchivements
            var achievementIds = unlockAchivements.Select(x => x.AchievementId).Distinct();
            var gameIds = unlockAchivements.Select(x => x.GameId).Distinct(); // Đổi tên từ gameId thành gameIds

            // Truy vấn danh sách Achivements dựa vào AchievementId và GameId
            var result = await _dbContext.Achivements
                .Where(a => achievementIds.Contains(a.AchivementId) && gameIds.Contains(a.GameId))
                .ToListAsync();

            return result;
        }
        public async Task<Achivement> GetAchivementFromUnlock(UnlockAchivement unlock)
        {
            return await _dbContext.Achivements.FirstOrDefaultAsync(c => c.AchivementId == unlock.AchievementId);
        }
        public async Task<string> UploadImageAndGetThumbnailLink(string imagePath)
        {
            try
            {
                // 1. Create credential
                var credential = GoogleCredential.FromFile(PathtoServiceAccountKeyfile)
                    .CreateScoped(new[] { DriveService.ScopeConstants.DriveFile });

                // 2. Initialize Drive service
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential
                });

                // 3. Prepare the file metadata
                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = Path.GetFileName(imagePath),
                    MimeType = "image/jpeg" // Adjust based on your file type (e.g., "image/png" for PNG files)
                };

                // 4. Upload the image file
                using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var request = service.Files.Create(
                        fileMetadata,
                        stream,
                        "image/jpeg" // Adjust MIME type based on your file
                    );

                    request.Fields = "id"; // We only need the file ID
                    var file = await request.UploadAsync();

                    if (file.Status != Google.Apis.Upload.UploadStatus.Completed)
                    {
                        throw new Exception("Failed to upload file to Google Drive.");
                    }

                    var fileId = request.ResponseBody.Id;

                    // 5. Set the file's permissions to public
                    var permission = new Google.Apis.Drive.v3.Data.Permission
                    {
                        Role = "reader",  // Viewer role
                        Type = "anyone"   // Anyone can access
                    };

                    var permissionRequest = service.Permissions.Create(permission, fileId);
                    permissionRequest.Fields = "id";
                    await permissionRequest.ExecuteAsync();

                    // 6. Return the public thumbnail link
                    return $"https://drive.google.com/thumbnail?id={fileId}&sz=w1000"; // Customize the size as needed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading image or generating thumbnail link: {ex.Message}");
                return null;
            }
        }


        public async Task<Achivement> AddAchivement(
            string idobject,
      string id,
      string gameid,
      string trigger,
      string description,
      string name,
      string unlockImagePath,
      string lockImagePath)
        {
            try
            {
                // Step 1: Upload images to Google Drive and get their links
                string unlockImageLink = await UploadImageAndGetThumbnailLink(unlockImagePath);
                string lockImageLink = await UploadImageAndGetThumbnailLink(lockImagePath);

                if (string.IsNullOrEmpty(unlockImageLink) || string.IsNullOrEmpty(lockImageLink))
                {
                    MessageBox.Show("Failed to upload images to Google Drive.");
                    return null;
                }

                // Step 2: Create a new achievement
                var achievement = new Achivement
                {
                    Id = idobject,
                    AchivementId = id,
                    GameId = gameid,
                    Name = name,
                    Description = description,
                    Trigger = trigger,
                    UnlockImageLink = unlockImageLink,
                    LockImageLink = lockImageLink
                };

                // Step 3: Save the achievement to the database (MongoDB)
                  _dbContext.Achivements.Add(achievement);
                await _dbContext.SaveChangesAsync();
                // Notify success
                MessageBox.Show("Achievement added successfully!");
                return achievement;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding achievement: {ex.Message}");
                return null;
            }
        }
        public async Task<Achivement> UpdateAchievement(
        string idobject,
        string id,
        string gameid,
        string trigger,
        string description,
        string name,
        string unlockImagePath,
        string lockImagePath,
        Achivement achievement)
        {
            try
            {
                // Step 1: Find the existing achievement
                var existingAchievement = await _dbContext.Achivements.FindAsync(idobject);
                if (existingAchievement == null)
                {
                    MessageBox.Show("Achievement not found.");
                    return null;
                }

                // Step 2: Update images only if new paths are provided
                if (!string.IsNullOrEmpty(unlockImagePath) && unlockImagePath != achievement.UnlockImageLink)
                {
                    if (!string.IsNullOrEmpty(existingAchievement.UnlockImageLink))
                    {
                        DeleteImageFile(existingAchievement.UnlockImageLink); // Delete old unlock image
                    }

                    string unlockImageLink = await UploadImageAndGetThumbnailLink(unlockImagePath);
                    if (string.IsNullOrEmpty(unlockImageLink))
                    {
                        MessageBox.Show("Failed to upload new unlock image to Google Drive.");
                        return null;
                    }
                    existingAchievement.UnlockImageLink = unlockImageLink; // Update with new image link
                }

                if (!string.IsNullOrEmpty(lockImagePath) && lockImagePath != achievement.LockImageLink)
                {
                    if (!string.IsNullOrEmpty(existingAchievement.LockImageLink))
                    {
                        DeleteImageFile(existingAchievement.LockImageLink); // Delete old lock image
                    }

                    string lockImageLink = await UploadImageAndGetThumbnailLink(lockImagePath);
                    if (string.IsNullOrEmpty(lockImageLink))
                    {
                        MessageBox.Show("Failed to upload new lock image to Google Drive.");
                        return null;
                    }
                    existingAchievement.LockImageLink = lockImageLink; // Update with new image link
                }

                // Step 3: Conditionally update other properties
                if (!string.IsNullOrWhiteSpace(id))
                {
                    existingAchievement.AchivementId = id;
                }

                if (!string.IsNullOrWhiteSpace(gameid))
                {
                    existingAchievement.GameId = gameid;
                }

                if (!string.IsNullOrWhiteSpace(trigger))
                {
                    existingAchievement.Trigger = trigger;
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    existingAchievement.Description = description;
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    existingAchievement.Name = name;
                }

                // Step 4: Update related UnlockAchivements if AchievementId changes
                var oldAchievementId = achievement.AchivementId;
                if (!string.IsNullOrWhiteSpace(id) && oldAchievementId != id)
                {
                    var unlockAchievements = _dbContext.UnlockAchivements
                        .Where(ua => ua.AchievementId == oldAchievementId)
                        .ToList();

                    foreach (var unlock in unlockAchievements)
                    {
                        unlock.AchievementId = id; // Update to the new AchievementId
                        unlock.GameId = gameid ?? unlock.GameId; // Update GameId only if provided
                    }

                    _dbContext.UnlockAchivements.UpdateRange(unlockAchievements);
                }

                // Step 5: Save changes to the database
                _dbContext.Achivements.Update(existingAchievement);
                await _dbContext.SaveChangesAsync();

                // Notify success
                MessageBox.Show("Achievement and related unlock records updated successfully!");
                return existingAchievement;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating achievement: {ex.Message}");
                return null;
            }
        }



        private void DeleteImageFile(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    File.Delete(imagePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting image file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public async Task DeleteAchievement(Achivement achievement)
        {
            try
            {
                // Delete associated image files
                DeleteImageFile(achievement.UnlockImageLink);
                DeleteImageFile(achievement.LockImageLink);

                // Remove related UnlockAchivements
                var unlockAchievements = _dbContext.UnlockAchivements
                    .Where(ua => ua.AchievementId == achievement.AchivementId)
                    .ToList();

                if (unlockAchievements.Any())
                {
                    _dbContext.UnlockAchivements.RemoveRange(unlockAchievements);
                }

                // Remove the achievement
                _dbContext.Achivements.Remove(achievement);

                // Save changes
                await _dbContext.SaveChangesAsync();

                // Notify success
                MessageBox.Show("Achievement and associated unlock records deleted successfully.",
                                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Handle any errors
                MessageBox.Show($"Error deleting achievement: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




    }
}
