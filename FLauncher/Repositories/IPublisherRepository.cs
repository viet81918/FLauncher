using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public interface IPublisherRepository
    {
        GamePublisher GetPublisherByGame(Game game);
        List<Update> getUpdatesForGame(Game game);
        GamePublisher GetPublisherByUser(User user);
    }
}
