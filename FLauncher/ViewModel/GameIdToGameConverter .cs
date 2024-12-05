using FLauncher.Model;
using FLauncher.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FLauncher.ViewModel
{
    public class GameIdToGameConverter : IValueConverter
    {
        private  IGameRepository _gameRepo;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gameIds = value as List<string>;
            if (gameIds == null) return null;

            var games = new List<Game>();
            foreach (var gameId in gameIds)
            {
                // Fetch the game based on the GameId (you might need to fetch it from a repository or database)
                var game = GetGameById(gameId);
                if (game != null)
                    games.Add(game);
            }

            return games;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        private Game GetGameById(string gameId)
        {
            _gameRepo = new GameRepository();
          return _gameRepo.GetGamesByGameID(gameId).Result;
        }
    }

}
