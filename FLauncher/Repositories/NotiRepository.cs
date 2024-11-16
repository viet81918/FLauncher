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
        public async Task<IEnumerable<Notification>> GetNotiforGamer(Gamer gamer)
        {
           return await NotiDAO.Instance.GetNotiforGamer(gamer);
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotiforGamer(Gamer gamer)
        {
            return await NotiDAO.Instance.GetUnreadNotiforGamer(gamer);
        }
    }
}
