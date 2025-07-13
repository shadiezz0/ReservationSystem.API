using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ReservationSystem.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace ReservationSystem.Application.Service.Auth
{
      public class TokenService : ITokenService
      {
            private readonly IConfiguration _config;

            public TokenService(IConfiguration config)
            {
                  _config = config;
            }

            public string GenerateAccessToken(User user, string role)
            {
                  var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Role, role)
                    };

                  var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                  var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                  var expires = DateTime.UtcNow.AddMinutes(15);

                  var token = new JwtSecurityToken(
                      issuer: _config["Jwt:Issuer"],
                      audience: _config["Jwt:Audience"],
                      claims: claims,
                      expires: expires,
                      signingCredentials: creds
                  );
                  return new JwtSecurityTokenHandler().WriteToken(token);
            }

            public string GenerateRefreshToken()
            {
                  return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            }


            public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
            {
                  var tokenValidationParameters = new TokenValidationParameters
                  {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = false, // We are allowing expired token to extract claims
                        ValidIssuer = _config["Jwt:Issuer"],
                        ValidAudience = _config["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]))
                  };

                  var tokenHandler = new JwtSecurityTokenHandler();
                  var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                  if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                      !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                  {
                        throw new SecurityTokenException("Invalid token");
                  }

                  return principal;
            }


      }


}
