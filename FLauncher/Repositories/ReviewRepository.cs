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
        public async Task<IEnumerable<Gamer>> GetGamerByReview(IEnumerable<Review> reviews)
        {
            return await ReviewDAO.Instance.GetGamerByReview(reviews);
        }

        public async Task<IEnumerable<Review>> GetReviewsByGame(Game game)
        {
           return await ReviewDAO.Instance.GetReviewsByGame(game);    
        }

      
    }
}
