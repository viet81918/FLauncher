using FLauncher.DAO;
using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public class GenreRepository : IGenresRepository
    {
        public async Task<IEnumerable<Genre>> GetGenres()
        {
            return await GenreDAO.Instance.GetGenres();   
        }

        public async Task<IEnumerable<Genre>> GetGenresFromGame(Game game)
        {
            return await GenreDAO.Instance.GetGenresFromGame(game);
        }
    }
}
