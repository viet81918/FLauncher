using FLauncher.Model;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> AddCategoryAsync(Gamer gamer, string nameCategories);


        Task AddGameToCategoryAsync(Category category, Game game);


      Task DeleteCategoryAsync(Category category);


        Task RemoveGameFromCategoryAsync(Category category, Game game);

        Task<IEnumerable<Category>> GetAllCategoriesByGamerAsync(Gamer gamer);

        Task<IEnumerable<Game>> GetAllGamesFromCategoryAsync(Category Category);
        Task<IEnumerable<Category>> GetCategoriesByGameAndGamerAsync(Gamer gamer, Game game);
    }
}
