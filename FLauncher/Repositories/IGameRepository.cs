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
        Task<IEnumerable<Game>> GetAllGame();
        void Download_game(Game game, String saveLocation, Gamer gamer);
        void Play_Game(Game game,Gamer gamer );
        void Upload_game(GamePublisher publisher,Game game, string selectedFilePath, string message);
        Task<IEnumerable<Game>> GetGameByInformation(string inputName, List<string> genres, string pubs);
    }
}
