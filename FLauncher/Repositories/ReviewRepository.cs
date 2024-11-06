using FLauncher.DAO;
using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        public List<Review> GetReviewsByGame(Game game)
        {
           return ReviewDAO.Instance.GetReviewsByGame(game);    
        }
    }
}
