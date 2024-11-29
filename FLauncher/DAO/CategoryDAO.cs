using FLauncher.DAO;
using FLauncher.Model;
using FLauncher.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FLauncher.Repositories
{
    public class CategoryDAO : SingletonBase<CategoryDAO>
    {

        private readonly FlauncherDbContext _dbContext;


        public CategoryDAO()
        {
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
        }
        public async Task<Category> AddCategoryAsync(Gamer gamer, string nameCategories)
        {
            var newCategory = new Category
            {

                Id = ObjectId.GenerateNewId().ToString(),
                GamerId = gamer.GamerId,
                NameCategories = nameCategories,
                GameIds = new List<string>()

            };

            _dbContext.Categories.Add(newCategory);
            await _dbContext.SaveChangesAsync();
            return newCategory; 
        }
        public async Task AddGameToCategoryAsync(Category category, Game game)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            }

            if (game == null)
            {
                throw new ArgumentNullException(nameof(game), "Game cannot be null.");
            }
           
            // Check if the game already exists in the category
            if (!category.GameIds.Contains(game.GameID))
            {
                
               category.GameIds.Add(game.GameID);
               
                _dbContext.Categories.Update(category);
                await _dbContext.SaveChangesAsync();

                MessageBox.Show($"Game '{game.GameID}' added to category '{category.NameCategories}'.");
            }
            else
            {
                MessageBox.Show($"Game '{game.GameID}' is already in category '{category.NameCategories}'.");
            }
        }
        public async Task DeleteCategoryAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            }
         
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            MessageBox.Show($"Category '{category.NameCategories}' deleted successfully.");
        }
        public async Task RemoveGameFromCategoryAsync(Category category, Game game)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            }

            if (game == null)
            {
                throw new ArgumentNullException(nameof(game), "Game cannot be null.");
            }

            // Check if the game exists in the category
            if (category.GameIds.Contains(game.GameID))
            {
                // Remove the game from the category
                category.GameIds.Remove(game.GameID);

                // Save changes to the database
                _dbContext.Categories.Update(category);
                await _dbContext.SaveChangesAsync();

                MessageBox.Show($"Game '{game.GameID}' removed from category '{category.NameCategories}'.");
            }
            else
            {
                MessageBox.Show($"Game '{game.GameID}' does not exist in category '{category.NameCategories}'.");
            }
        }
        public async Task<IEnumerable<Category>> GetAllCategoriesByGamerAsync(Gamer gamer)
        {
            if (string.IsNullOrWhiteSpace(gamer.GamerId))
            {
                throw new ArgumentNullException(nameof(gamer.GamerId), "Gamer ID cannot be null or empty.");
            }

            // Fetch categories where GamerId matches the provided gamerId
            var categories = await _dbContext.Categories
                .Where(category => category.GamerId == gamer.GamerId)
                .ToListAsync();

            return categories;
        }
        public async Task<IEnumerable<Category>> GetCategoriesByGameAndGamerAsync(Gamer gamer, Game game)
        {
            if (string.IsNullOrWhiteSpace(gamer.GamerId))
            {
                throw new ArgumentNullException(nameof(gamer.GamerId), "Gamer ID cannot be null or empty.");
            }

            if (game == null || string.IsNullOrWhiteSpace(game.GameID))
            {
                throw new ArgumentNullException(nameof(game), "Game cannot be null or have an empty ID.");
            }

            // Fetch categories where GamerId matches the provided gamerId
            // and the GameId exists in the category's GameIds collection
            var categories = await _dbContext.Categories
                .Where(category => category.GamerId == gamer.GamerId && category.GameIds.Contains(game.GameID))
                .ToListAsync();

            return categories;
        }

        public async Task<IEnumerable<Game>> GetAllGamesFromCategoryAsync(Category Category)
        {
            if (string.IsNullOrWhiteSpace(Category.NameCategories))
            {
                throw new ArgumentNullException(nameof(Category.NameCategories), "Category name cannot be null or empty.");
            }

            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.NameCategories == Category.NameCategories);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with name '{Category.NameCategories}' not found.");
            }

            // Fetch games based on GameIds in the category
            var games = await _dbContext.Games
                .Where(game => category.GameIds.Contains(game.GameID))
                .ToListAsync();

            return games;
        }





    }
}