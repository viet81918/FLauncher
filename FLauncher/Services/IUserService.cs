using FLauncher.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Services
{
    public interface IUserService
    {
        List<User> GetUsers();
        User GetUserByEmailPass(string email, string pass);
    }
}
