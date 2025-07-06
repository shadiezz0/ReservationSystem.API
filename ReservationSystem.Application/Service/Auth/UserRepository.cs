using ReservationSystem.Application.IService.IAuth;

namespace ReservationSystem.Application.Service.Auth
{
      public class UserRepository : IUserRepository
      {
            private readonly IGenericRepository<User> _repo;

            public UserRepository(IUnitOfWork unitOfWork)
            {
                  _repo = unitOfWork.Repository<User>();
            }

            public async Task<User?> GetByEmailAsync(string email)
            {
                  var users = await _repo.GetAllAsync();
                  return users.FirstOrDefault(u => u.Email == email);
            }

            public async Task AddAsync(User user)
            {
                  await _repo.AddAsync(user);
            }
      }


}
