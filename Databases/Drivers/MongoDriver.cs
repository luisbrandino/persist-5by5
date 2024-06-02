using MongoDB.Driver;
using System.Reflection;

namespace Databases.Drivers
{
    public class MongoDriver : IDatabaseDriver
    {
        private IMongoClient _client;
        private IMongoDatabase _database;
        private string _connectionString;

        public MongoDriver(string databaseName, string user, string password)
        {
            _connectionString = "mongodb://";

            if (user != null && password != null)
            {
                user = Uri.EscapeDataString(user);
                password = Uri.EscapeDataString(password);

                _connectionString += $"{user}:{password}@";
            }

            _connectionString += "localhost:27017";
            _client = new MongoClient(_connectionString);
            _database = _client.GetDatabase(databaseName);
        }

        public T? Find<T>(int id) where T : Model, new()
        {
            IMongoCollection<T> collection = _database.GetCollection<T>(typeof(T).GetCustomAttribute<CollectionAttribute>()?.CollectionName);

            var filter = Builders<T>.Filter.Eq("_id", id);

            return collection.Find(filter).FirstOrDefault();
        }

        public List<T> FindAll<T>() where T : Model, new()
        {
            IMongoCollection<T> collection = _database.GetCollection<T>(typeof(T).GetCustomAttribute<CollectionAttribute>()?.CollectionName);

            return collection.Find(FilterDefinition<T>.Empty).ToList();
        }

        public void Insert(Model model)
        {
            IMongoCollection<Model> collection = _database.GetCollection<Model>(model.Collection());

            collection.InsertOne(model);
        }

        public void InsertMany(List<Model> models)
        {
            if (models.Count == 0)
                return;

            IMongoCollection<Model> collection = _database.GetCollection<Model>(models.First().Collection());

            collection.InsertMany(models);
        }
    }
}
