using System.Collections.Concurrent;
using System.Linq.Expressions;
using eClaims.Core.Entities;
using eClaims.Core.Interfaces;
using System;

namespace eClaims.Infrastructure.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        private static readonly ConcurrentDictionary<string, T> _store = new();



        public Task<IReadOnlyList<T>> GetAllAsync()
        {
            var values = _store.Values.ToList();
            return Task.FromResult<IReadOnlyList<T>>(values);
        }

        public Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            var func = predicate.Compile();
            var values = _store.Values.Where(func).ToList();
            return Task.FromResult<IReadOnlyList<T>>(values);
        }

        public Task<T?> GetByIdAsync(string id)
        {
            _store.TryGetValue(id, out var value);
            return Task.FromResult(value);
        }

        public Task<T> AddAsync(T entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
            }
            entity.CreatedAt = DateTime.UtcNow;
            _store.TryAdd(entity.Id, entity);
            return Task.FromResult(entity);
        }

        public Task UpdateAsync(T entity)
        {
            if (_store.ContainsKey(entity.Id))
            {
               entity.UpdatedAt = DateTime.UtcNow;
               _store[entity.Id] = entity;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            _store.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }
}
