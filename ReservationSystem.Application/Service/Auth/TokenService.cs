using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ReservationSystem.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace ReservationSystem.Application.Service.Auth
{
      public class TokenService : ITokenService
      {
            private readonly IConfiguration _config;

            public TokenService(IConfiguration config) => _config = config;

            public string GenerateToken(User user)
            {
                  var claims = new[]
                  {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role.Name)
                    };

                  var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
                  var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                  var token = new JwtSecurityToken(
                      _config["Jwt:Issuer"],
                      _config["Jwt:Audience"],
                      claims,
                      expires: DateTime.Now.AddHours(1),
                      signingCredentials: creds
                  );

                  return new JwtSecurityTokenHandler().WriteToken(token);
            }
      }


}
