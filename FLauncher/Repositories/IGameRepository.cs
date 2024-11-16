﻿using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public interface IGameRepository
    {
      Task<IEnumerable<Game>> GetTopGames();
        void Download_game(Game game, String saveLocation, Gamer gamer);
    }
}