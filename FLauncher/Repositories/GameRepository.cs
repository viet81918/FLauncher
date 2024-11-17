using FLauncher.DAO;
using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public class GameRepository : IGameRepository
    {
        public void Download_game(Game game, string saveLocation, Gamer gamer)
        {
            GameDAO.Instance.DownloadRarFromFolder(game, saveLocation, gamer);
        }

        public async Task<IEnumerable<Game>> GetTopGames()
        {
           return await GameDAO.Instance.GetTopGames();  
        }

        public void Play_Game(Game game, Gamer gamer)
        {
            GameDAO.Instance.PlayGame(game, gamer); 
        }
    }
}
