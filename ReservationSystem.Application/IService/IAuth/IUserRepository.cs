﻿
namespace ReservationSystem.Application.IService.IAuth
{
      public interface IUserRepository
      {
            Task<User?> GetByEmailAsync(string email);
            Task AddAsync(User user);
      }

}
