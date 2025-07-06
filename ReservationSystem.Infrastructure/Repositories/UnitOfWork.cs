using ReservationSystem.Domain.Interfaces;
using ReservationSystem.Infrastructure.Context;

namespace ReservationSystem.Infrastructure.Repositories
{
      public class UnitOfWork : IUnitOfWork , IDisposable
      {
            private readonly AppDbContext _context;
            private readonly Dictionary<Type, object> _repositories = new();

            public UnitOfWork(AppDbContext context)
            {
                  _context = context;
            }

            public IGenericRepository<T> Repository<T>() where T : class
            {
                  var type = typeof(T);

                  if (!_repositories.TryGetValue(type, out var repository))
                  {
                        var repoInstance = new GenericRepository<T>(_context);
                        _repositories[type] = repoInstance;
                        return repoInstance;
                  }

                  return (IGenericRepository<T>)repository!;
            }

            public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

            public void Dispose() =>  _context.Dispose();
            
      }
}
