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
        Task<GamePublisher> GetPublisherByGame(Game game);
        Task<IEnumerable<Update>> getUpdatesForGame(Game game);
        GamePublisher GetPublisherByUser(User user);
        Task <IEnumerable<GamePublisher>> GetTopPublishersAsync();
    }
}
