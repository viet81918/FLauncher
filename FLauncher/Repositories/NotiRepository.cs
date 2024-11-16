using FLauncher.DAO;
using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public class NotiRepository : INotiRepository
    {
        public List<Notification> GetNotiforGamer(Gamer gamer)
        {
           return NotiDAO.Instance.GetNotiforGamer(gamer);
        }

        public List<Notification> GetUnreadNotiforGamer(Gamer gamer)
        {
            return NotiDAO.Instance.GetUnreadNotiforGamer(gamer);
        }
    }
}
