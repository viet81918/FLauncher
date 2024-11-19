using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public interface IGamerRepository
    {
        Gamer GetGamerByUser(User user);

        Gamer GetGamerById(string gamerId);

        Task<IEnumerable<Gamer>> GetGamersByIds(List<string> gamerIds);

        Task<IEnumerable<Gamer>> GetGamersFromGame(Game game);

    }
}