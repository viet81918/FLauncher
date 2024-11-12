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

        Task<Gamer> GetGamerById(string gamerId);
       
    }
}
