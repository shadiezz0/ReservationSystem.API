using ReservationSystem.Domain.Entities;

namespace ReservationSystem.Domain.Interfaces
{
      public interface IUnitOfWork
      {
            IRepository<T> Repository<T>() where T : class;
            Task<int> SaveAsync();
      }
}

