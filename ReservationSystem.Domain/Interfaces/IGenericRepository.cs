using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace ReservationSystem.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(
           int id,
           Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
       );

        Task<IEnumerable<T>> GetAllAsync(
           Expression<Func<T, bool>>? predicate = null,
           Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
       );

        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        Task<T?> FindOneAsync(
           Expression<Func<T, bool>> predicate,
           Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
       );
    }
}
