using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public  interface INotiReposotory
    {
        List<Notification> GetNotiforGamer(Gamer gamer);
        List<Notification> GetUnreadNotiforGamer(Gamer gamer);
    }
}
