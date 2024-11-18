using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public interface IMessageRepository
    {
        List<Message> GetMessages(string senderId, string receiverId);
        void SendMessage(Message message);
    }
}
