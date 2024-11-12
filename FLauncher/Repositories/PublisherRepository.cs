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
        public GamePublisher GetPublisherByGame(Game game)
        {
           return PublisherDAO.Instance.GetPublisherByGame(game);
        }

        public List<Update> getUpdatesForGame(Game game)
        {
            return PublisherDAO.Instance.getUpdatesForGame(game);
        }
        public GamePublisher GetPublisherByUser(User user)
        {
            return PublisherDAO.Instance.GetPublisherByUser(user);
        }
    }
}
