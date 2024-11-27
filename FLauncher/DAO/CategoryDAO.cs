using FLauncher.Model;
using MongoDB.Driver;

namespace FLauncher.DAO
{
    public class CategoryDAO : SingletonBase<CategoryDAO>
    {
        private readonly IMongoCollection<Category> _categoriesCollection;
        private readonly IMongoCollection<Game> _gameCollection;

        public CategoryDAO()
        {
            // MongoDB connection setup
            var connectionString = "mongodb://localhost:27017"; // Adjust this to your MongoDB connection string
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("FPT"); // Assuming database name is FPT
            _gameCollection = database.GetCollection<Game>("Games");
            _categoriesCollection = database.GetCollection<Category>("Category"); // Assuming collection name is "Category"
        }

        // Get all categories from the MongoDB database
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _categoriesCollection.Find(FilterDefinition<Category>.Empty).ToListAsync();
        }

        // Get a category by its name
        public async Task<Category> GetCategoryByNameAsync(string categoryName)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.NameCategories, categoryName);
            return await _categoriesCollection.Find(filter).FirstOrDefaultAsync();
        }

        // Add a new category to the database
        public async Task AddCategoryAsync(string categoryName, string gamerId)
        {
            var existingCategory = await GetCategoryByNameAsync(categoryName);
            if (existingCategory == null)
            {
                var category = new Category
                {
                    NameCategories = categoryName,
                    GamerId = gamerId
                };

                await _categoriesCollection.InsertOneAsync(category);
            }
        }

        // Delete a category by name
        public async Task DeleteCategoryAsync(string categoryName)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.NameCategories, categoryName);
            await _categoriesCollection.DeleteOneAsync(filter);
        }

        // Check if a category exists in the database by name
        public async Task<bool> CategoryExistsAsync(string categoryName)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.NameCategories, categoryName);
            var count = await _categoriesCollection.CountDocumentsAsync(filter);
            return count > 0;
        }

        // Add a game ID to a category's list of games
        public async Task AddGameToCategoryAsync(string categoryName, string gameId)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.NameCategories, categoryName);
            var update = Builders<Category>.Update.AddToSet(c => c.GameIds, gameId);
            await _categoriesCollection.UpdateOneAsync(filter, update);
        }

        public async Task<List<Game>> GetGamesByCategoryAsync(string categoryName)
        {
            var category = await _categoriesCollection
                .Find(c => c.NameCategories == categoryName)
                .FirstOrDefaultAsync();


            if (category != null && category.GameIds != null)
            {

                var games = await _gameCollection
                .Find(g => category.GameIds.Contains(g.GameID))
                .ToListAsync();


                return games;
            }

            return new List<Game>();
        }


        public async Task RemoveGameFromCategoryAsync(string categoryName, string gameId)
        {
            // Define the filter to find the category by name
            var filter = Builders<Category>.Filter.Eq(c => c.NameCategories, categoryName);

            // Define the update operation to remove the gameId from the GameIds list
            var update = Builders<Category>.Update.Pull(c => c.GameIds, gameId);

            // Update the category document to remove the gameId from the GameIds list
            await _categoriesCollection.UpdateOneAsync(filter, update);
        }

    }
}
