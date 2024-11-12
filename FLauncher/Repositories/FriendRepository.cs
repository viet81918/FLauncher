using FLauncher.DAO;
using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        public List<Friend> GetFriendInvitationsforGamer(Gamer gamer)
        {
           return FriendDAO.Instance.GetFriendInvitationsforGamer(gamer);
        }

        public List<Gamer> GetFriendWithTheSameGame(Game game, Gamer gamer)
        {
            return FriendDAO.Instance.GetFriendWithTheSameGame(game, gamer);
        }
    }
}
