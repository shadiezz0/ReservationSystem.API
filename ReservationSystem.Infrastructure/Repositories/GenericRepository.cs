using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using ReservationSystem.Domain.Interfaces;
using ReservationSystem.Infrastructure.Context;

namespace ReservationSystem.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
                            Expression<Func<T, bool>>? predicate = null,
                            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
                        )
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
                query = query.Where(predicate);

            if (include != null)
                query = include(query);

            return await query.ToListAsync();
        }


        public async Task<T?> FindOneAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
        )
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> GetByIdAsync(
                                        int id,
                                        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
                                         )
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Delete(T entity) => _dbSet.Remove(entity);
        public void DeleteRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();

    }
}
