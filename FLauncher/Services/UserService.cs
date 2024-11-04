using FLauncher.Model;
using FLauncher.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Services
{
    public class UserService : IUserService // Fix typo: IUserrService to IUserService
    {
        private readonly IUserRepository userRepository;
        public UserService()
        {
            // Initialize repository; consider using dependency injection here
            userRepository = new UserRepository(); // Replace with actual repository implementation
        }

        public  User GetUserByEmailPass(string email, string pass)
        {
           return   userRepository.GetUserByEmailPass(email, pass);
        }

        public  List<User> GetUsers()
        {

           return   userRepository.GetUsers();
        }
    }

}
