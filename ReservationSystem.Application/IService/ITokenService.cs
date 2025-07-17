

using System.Security.Claims;

public interface ITokenService
{
    string GenerateAccessToken(User user, string role);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    void SaveRefreshToken(string email, string refreshToken);
    string GetSavedRefreshToken(string email);
    void RemoveRefreshToken(string email);
}
