using eClaims.Core.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace eClaims.Infrastructure.Data
{
    public class ClaimContext
    {
        private readonly IMongoDatabase _database;

        public ClaimContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Claim> Claims => _database.GetCollection<Claim>("Claims");
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    }
}
