using FLauncher.DAO;
using FLauncher.Model;

namespace FLauncher.Repositories
{
    public class GameRepository : IGameRepository
    {
        public void Download_game(Game game, string saveLocation, Gamer gamer)
        {
            GameDAO.Instance.DownloadRarFromLink(game, saveLocation, gamer);
        }

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

        public void Play_Game(Game game, Gamer gamer)
        {
            GameDAO.Instance.PlayGame(game, gamer);
        }

        public void Upload_game(GamePublisher publisher, Game game, string selectedFilePath, string message)
        {
            GameDAO.Instance.Update_Game(publisher, game, selectedFilePath, message);
        }

    }
}
