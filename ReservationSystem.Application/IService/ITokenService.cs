

using System.Security.Claims;

public interface ITokenService
{
    string GenerateAccessToken(User user, string role);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    Task SaveRefreshTokenAsync(string email, string refreshToken);
    Task<string?> GetSavedRefreshTokenAsync(string email);
    Task RemoveRefreshTokenAsync(string email);
}
