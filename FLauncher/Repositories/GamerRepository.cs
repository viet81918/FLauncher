﻿using FLauncher.DAO;
using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public class GamerRepository : IGamerRepository
    {
        private readonly GamerDAO _gamerDAO;
        public GamerRepository(){
            _gamerDAO = GamerDAO.Instance;

            }

        public Gamer GetGamerByUser(User user)
        {
           return GamerDAO.Instance.GetGamerByUser(user);
        }
        public Gamer GetGamerById(string gamerId) {
            return  _gamerDAO.GetGamerById(gamerId);

        }
    }
}
