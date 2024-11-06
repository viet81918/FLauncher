using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public interface IFriendRepository
    {
        List<Friend> GetFriendInvitationsforGamer(Gamer gamer);
    }
}
