using FLauncher.DAO;
using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public class PublisherRepository : IPublisherRepository
    {
        public async Task <GamePublisher> GetPublisherByGame(Game game)
        {
           return await PublisherDAO.Instance.GetPublisherByGame(game);
        }

        public async Task<IEnumerable<Update>> getUpdatesForGame(Game game)
        {
            return await PublisherDAO.Instance.getUpdatesForGame(game);
        }
        public GamePublisher GetPublisherByUser(User user)
        {
            return PublisherDAO.Instance.GetPublisherByUser(user);
        }
    }
}
