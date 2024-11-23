using FLauncher.DAO;
using FLauncher.Model;

namespace FLauncher.Repositories
{
    public class GameRepository : IGameRepository
    {



        public async Task<Achivement> GetAchivementFromUnlock(UnlockAchivement unlock)
        {
            return await GameDAO.Instance.GetAchivementFromUnlock(unlock);
        }

        public async Task<IEnumerable<Achivement>> GetAchivementsFromUnlocks(IEnumerable<UnlockAchivement> unlockAchivements)
        {
            return await GameDAO.Instance.GetAchivementsFromUnlocks(unlockAchivements);
        }

        public async Task<IEnumerable<Achivement>> GetAchivesFromGame(Game game)
        {
            return await GameDAO.Instance.GetAchivementFromGame(game);
        }

        public async Task<IEnumerable<Game>> GetGamesByGamer(Gamer gamer)
        {
            return await GameDAO.Instance.GetGamesByGamer(gamer);
        }

        public async Task<IEnumerable<Achivement>> GetLockAchivement(IEnumerable<Achivement> achivements, Gamer gamer)
        {
            return await GameDAO.Instance.GetLockAchivement(achivements, gamer);
        }

        public async Task<IEnumerable<Game>> GetTopGames()
        {
            return await GameDAO.Instance.GetTopGames();
        }

        public async Task<IEnumerable<UnlockAchivement>> GetUnlockAchivementsFromGame(IEnumerable<Achivement> achivement, Gamer gamer)
        {
            return await GameDAO.Instance.GetUnlockAchivements(achivement, gamer);
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
    }
}
