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
            GameDAO.Instance.DownloadRarFromLink(game, saveLocation, gamer);
        }

        public async Task<IEnumerable<Game>> GetTopGames()
        {
           return await GameDAO.Instance.GetTopGames();  
        }
        public async Task<IEnumerable<Game>> GetAllGame()
        {
            return await GameDAO.Instance.GetAllGame();
        }
        public void Play_Game(Game game, Gamer gamer)
        {
            GameDAO.Instance.PlayGame(game, gamer); 
        }

        public void Upload_game(GamePublisher publisher,Game game, string selectedFilePath, string message)
        {
            GameDAO.Instance.Update_Game(publisher, game, selectedFilePath, message);
        }        
        public async Task<IEnumerable<Game>> GetGameByInformation(string inputName, List<string> genres, string pubs)
        {
            return await GameDAO.Instance.GetGameByInformation(inputName, genres, pubs);
        }
    }
}
