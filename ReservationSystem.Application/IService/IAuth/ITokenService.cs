

using System.Security.Claims;

public interface ITokenService
{
      string GenerateAccessToken(User user, string role);
      string GenerateRefreshToken();
      ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
