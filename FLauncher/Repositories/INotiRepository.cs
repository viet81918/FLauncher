using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public  interface INotiRepository
    {
        Task<IEnumerable<Notification>> GetNotiforGamer(Gamer gamer);
        Task<IEnumerable<Notification>> GetUnreadNotiforGamer(Gamer gamer);
    }
}
