using ReservationSystem.Domain.Entities;

public interface ITokenService
{
      string GenerateToken(User user);
}
