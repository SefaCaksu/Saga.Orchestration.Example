using System;
using MongoDB.Driver;

namespace Stock.API.Services
{
	public class MongoDBService
	{
        readonly IMongoDatabase _mongoDatabase;

        public MongoDBService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            _mongoDatabase = client.GetDatabase("StockAPIDB");
        }

        public IMongoCollection<T> GetCollection<T>() => _mongoDatabase.GetCollection<T>(typeof(T).Name.ToLowerInvariant());
    }
}

