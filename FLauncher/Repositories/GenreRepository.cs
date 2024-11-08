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
        public List<Genre> GetGenres()
        {
            return GenreDAO.Instance.GetGenres();   
        }

        public List<Genre> GetGenresFromGame(Game game)
        {
            return GenreDAO.Instance.GetGenresFromGame(game);
        }
    }
}
