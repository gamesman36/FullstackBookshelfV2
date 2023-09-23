using MongoDB.Driver;
using FullstackBookshelf.Model;

namespace FullstackBookshelf
{
    public class MongoDBConnect
    {
        private readonly IMongoCollection<Book> _collection;

        public MongoDBConnect(string databaseName, string collectionName)
        {
            string connectionString = GetConnectionStringFromEnvironmentVariable();

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<Book>(collectionName);
        }
        private string GetConnectionStringFromEnvironmentVariable()
        {
            string connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("DATABASE_URL environment variable not found.");
            }

            return connectionString;
        }

        public async Task InsertBookAsync(Book book)
        {
            try
            {
                await _collection.InsertOneAsync(book);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Couldn't insert book: " + ex.Message);
            }
        }

        public async Task DeleteBookAsync(int id)
        {
            try
            {
                var filter = Builders<Book>.Filter.Eq(book => book.Id, id);
                await _collection.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Couldn't delete book: " + ex.Message);
            }
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            try
            {
                var filter = Builders<Book>.Filter.Empty;
                var books = await _collection.Find(filter).ToListAsync();
                return books;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Couldn't get all books: " + ex.Message);
                return new List<Book>();
            }
        }
    }
}