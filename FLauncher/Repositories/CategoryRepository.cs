using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public async Task<Category> AddCategoryAsync(Gamer gamer, string nameCategories)
        {
           return await CategoryDAO.Instance.AddCategoryAsync(gamer, nameCategories);
        }

        public async Task AddGameToCategoryAsync(Category category, Game game)
        {
           await CategoryDAO.Instance.AddGameToCategoryAsync(category, game);
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            await CategoryDAO.Instance.DeleteCategoryAsync(category);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesByGamerAsync(Gamer gamer)
        {
            return await CategoryDAO.Instance.GetAllCategoriesByGamerAsync(gamer);
        }

        public async Task<IEnumerable<Game>> GetAllGamesFromCategoryAsync(Category Category)
        {
            return await CategoryDAO.Instance.GetAllGamesFromCategoryAsync(Category);
        }

        public async Task<IEnumerable<Category>> GetCategoriesByGameAndGamerAsync(Gamer gamer, Game game)
        {
           return await CategoryDAO.Instance.GetCategoriesByGameAndGamerAsync (gamer, game);
        }

        public async Task RemoveGameFromCategoryAsync(Category category, Game game)
        {
             await CategoryDAO.Instance.RemoveGameFromCategoryAsync (category, game);
        }
    }
}
