using FLauncher.DAO;
using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessageDAO _messageDAO;
        public MessageRepository()
        {
            _messageDAO = MessageDAO.Instance; // Assuming FriendDAO follows Singleton pattern
        }
        public List<Message> GetMessages(string? senderId, string? receiverId)
        {
            return _messageDAO.GetMessages( senderId,receiverId);
        }
        public void SendMessage(Message message)
        {
            _messageDAO.SendMessage( message );
        }
    }
}
