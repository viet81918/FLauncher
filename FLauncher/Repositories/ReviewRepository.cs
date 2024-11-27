using FLauncher.DAO;
using FLauncher.Model;

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
