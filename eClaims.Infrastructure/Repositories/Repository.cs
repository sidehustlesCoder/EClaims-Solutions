using System.Linq.Expressions;
using eClaims.Core.Entities;
using eClaims.Core.Interfaces;
using eClaims.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace eClaims.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;

        public Repository(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<T>(typeof(T).Name + "s"); // Simple pluralization
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _collection.Find(p => true).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, id);
            return await _collection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, entity.Id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}
