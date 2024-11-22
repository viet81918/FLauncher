using FLauncher.DAO;
using FLauncher.Model;

namespace FLauncher.Repositories
{
    public class GamerRepository : IGamerRepository
    {
        private readonly GamerDAO _gamerDAO;
        public GamerRepository()
        {
            _gamerDAO = GamerDAO.Instance;

        }

        public Gamer GetGamerByUser(User user)
        {
            return _gamerDAO.GetGamerByUser(user);
        }
        public Gamer GetGamerById(string gamerId)
        {
            return _gamerDAO.GetGamerById(gamerId);

        }

        public async Task<IEnumerable<Gamer>> GetGamersByIds(List<string> gamerIds)
        {
            return await _gamerDAO.GetGamersByIds(gamerIds);
        }

        public async Task<IEnumerable<Gamer>> GetGamersFromGame(Game game)
        {
            return await _gamerDAO.GetGamersFromGame(game);

        }

        public async Task<bool> IsUpdate(Game game, Gamer gamer)
        {
         return await GamerDAO.Instance.IsUpdate(game, gamer);
        }
       
    }
}
