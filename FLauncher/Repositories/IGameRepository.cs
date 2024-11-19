using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public interface IGameRepository
    {
      Task<IEnumerable<Game>> GetTopGames();
        void Download_game(Game game, String saveLocation, Gamer gamer);
        void Play_Game(Game game,Gamer gamer );
        void Upload_game(GamePublisher publisher,Game game, string selectedFilePath, string message);
        Task<IEnumerable<Achivement>> GetAchivesFromGame(Game game);
        Task<IEnumerable<UnlockAchivement>> GetUnlockAchivementsFromGame(IEnumerable<Achivement> achivements , Gamer gamer) ;
        Task<IEnumerable<Achivement>> GetAchivementsFromUnlocks(IEnumerable<UnlockAchivement> unlockAchivements);
        Task<Achivement> GetAchivementFromUnlock(UnlockAchivement unlock);
        Task<IEnumerable<Achivement>> GetLockAchivement(IEnumerable<Achivement> achivements, Gamer gamer);
        Task<bool> IsBuyGame(Game game, Gamer gamer);
        Task<bool> IsPublishGame(Game game, GamePublisher publisher
            );
        Task<bool> isDownload(Game game, Gamer gamer);

    }
}
