using FLauncher.DAO;
using FLauncher.Model;
using MongoDB.Bson;

namespace FLauncher.Repositories
{
    public class GameRepository : IGameRepository
    {



        public async Task<Achivement> GetAchivementFromUnlock(UnlockAchivement unlock)
        {
            return await AchivementDAO.Instance.GetAchivementFromUnlock(unlock);
        }

        public async Task<IEnumerable<Achivement>> GetAchivementsFromUnlocks(IEnumerable<UnlockAchivement> unlockAchivements)
        {
            return await AchivementDAO.Instance.GetAchivementsFromUnlocks(unlockAchivements);
        }

        public async Task<IEnumerable<Achivement>> GetAchivesFromGame(Game game)
        {
            return await AchivementDAO.Instance.GetAchivementFromGame(game);
        }

        public async Task<IEnumerable<Game>> GetGamesByGamer(Gamer gamer)
        {
            return await GameDAO.Instance.GetGamesByGamer(gamer);
        }

        public async Task<IEnumerable<Achivement>> GetLockAchivement(IEnumerable<Achivement> achivements, Gamer gamer)
        {
            return await AchivementDAO.Instance.GetLockAchivement(achivements, gamer);
        }

        public async Task<IEnumerable<Game>> GetTopGames()
        {
            return await GameDAO.Instance.GetTopGames();
        }

        public async Task<IEnumerable<UnlockAchivement>> GetUnlockAchivementsFromGame(IEnumerable<Achivement> achivement, Gamer gamer)
        {
            return await AchivementDAO.Instance.GetUnlockAchivements(achivement, gamer);
        }

        public async Task<bool> IsBuyGame(Game game, Gamer gamer)
        {
            return await GameDAO.Instance.IsBuyGame(game, gamer);
        }

        public async Task<bool> isDownload(Game game, Gamer gamer)
        {
            return await GameDAO.Instance.isDownload(game, gamer);
        }

        public async Task<bool> IsPublishGame(Game game, GamePublisher publisher)
        {
            return await GameDAO.Instance.IsPublishGame(game, publisher);
        }


        public async Task Uninstall_Game(Gamer gamer, Game game)
        {
            await GameDAO.Instance.Uninstall_Game(gamer, game);
        }



        public async Task Download_game(Game game, string saveLocation, Gamer gamer)
        {
            await GameDAO.Instance.DownloadRarFromLink(game, saveLocation, gamer);
        }

        public async Task Play_Game(Game game, Gamer gamer)
        {
            await GameDAO.Instance.PlayGame(game, gamer);
        }

        public async Task Upload_game(GamePublisher publisher, Game game, string selectedFilePath, string message)
        {

            await GameDAO.Instance.Update_Game(publisher, game, selectedFilePath, message);
        }



        public async Task Reinstall(Game game, Gamer gamer)

        {
            await GameDAO.Instance.Reinstall(game, gamer);
        }
        public async Task<IEnumerable<TrackingRecords>> GetTrackingFromGamerGame(Gamer gamer, Game game)
        {
            return await TrackingDAO.Instance.GetTrackingFromGamerGame(gamer, game);
        }

        public async Task<IEnumerable<Game>> GetAllGame()
        {
            return await GameDAO.Instance.GetAllGame();
        }
        public async Task<IEnumerable<Game>> GetGameByInformation(string inputName, List<string> genres, string pubs)
        {
            return await GameDAO.Instance.GetGameByInformation(inputName, genres, pubs);
        }

        public async Task<IEnumerable<Game>> GetGamesByPublisher(GamePublisher publisher)
        {
            return await GameDAO.Instance.GetGamesByPublisher(publisher);
        }

        public  async Task <TrackingPlayers> GetTrackingFromGame(Game game)
        {
          return   await TrackingDAO.Instance.GetTrackingFromGame(game);
        }

        public async Task<Achivement> AddAchivement(string idobject, string id ,string gameid, string trigger, string description, string name, string unlockImagePath, string lockImagePath)
        {
          return  await AchivementDAO.Instance.AddAchivement(idobject,id, gameid ,trigger, description, name, unlockImagePath, lockImagePath);
        }

        public async Task DeleteAchievement(Achivement achievement)
        {
            await AchivementDAO.Instance.DeleteAchievement(achievement);
        }

        public async Task<Achivement> UpdateAchievement(string idobject, string id, string gameid, string trigger, string description, string name, string unlockImagePath, string lockImagePath, Achivement achievement)
        {
             return await AchivementDAO.Instance.UpdateAchievement(idobject, id, gameid, trigger, description, name, unlockImagePath, lockImagePath, achievement);
        }
        public async Task<Game> GetGamesByGameID(String Id)
        {
            return await GameDAO.Instance.GetGamesByGameID(Id);
        }
    }
}
