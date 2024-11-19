﻿using FLauncher.Model;
using FLauncher.Services;
using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.DAO
{

    public class MessageDAO : SingletonBase<MessageDAO>
    {
        private readonly FlauncherDbContext _dbContext;
        public MessageDAO()
        {

            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
        }
        public List<Message> GetMessages(string? senderId, string? receiverId)
        {
             
            var messages = _dbContext.Messages
                .Where(m => (m.IdSender == senderId && m.IdReceiver == receiverId) ||
                            (m.IdSender == receiverId && m.IdReceiver == senderId))
                .OrderBy(m => m.TimeString)
                .ToList();

            // Xác định xem tin nhắn có phải của người dùng hiện tại không
            foreach (var message in messages)
            {
                message.IsSenderCurrentUser = message.IdSender == senderId;
            }

            return messages;
        }

        /*
        public List<Message> GetMessages(string senderId, string receiverId)
        {
            // Lấy tất cả tin nhắn giữa người gửi và người nhận
            return _dbContext.Messages
                .Where(m => (m.IdSender == senderId && m.IdReceiver == receiverId) ||
                            (m.IdSender == receiverId && m.IdReceiver == senderId))
                .OrderBy(m => m.TimeString) // Sắp xếp theo thời gian
                .ToList();
        }
        /*
         public List<Message> GetMessages(Gamer gamer)
        {
            return _dbContext.Messages
                .Where(m => m.IdSender == gamer.Id || m.IdReceiver == gamer.Id) // Lọc các tin nhắn có IdSender hoặc IdReceiver là Id của gamer
                .OrderBy(m => m.TimeString) // Sắp xếp theo thời gian
                .ToList();
        }
         */

        public void SendMessage(Message message)
        {
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();
        }
    }
}