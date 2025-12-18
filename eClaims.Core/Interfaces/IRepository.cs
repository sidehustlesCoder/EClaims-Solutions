using System.Linq.Expressions;
using eClaims.Core.Entities;

namespace eClaims.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(string id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
    }
}
