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
        private readonly IGenericRepository<Role> _role;

        public TokenService(IConfiguration config, IGenericRepository<Role> role)
        {
            _config = config;
            _role = role;
        }

        public async Task<string> GenerateToken(User user)
        {
            var role = await _role.FindOneAsync(r => r.Id == user.RoleId);
            if (role == null)
            { throw new Exception("Role not found for the user."); }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role.Name)
            };

            if (claims == null || !claims.Any())
            { throw new Exception("Claims could not be created for the user."); }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            if (key == null)
            { throw new Exception("JWT Key is not configured properly."); }

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            if (creds == null)
            { throw new Exception("Signing credentials could not be created."); }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );
            if (token == null)
            { throw new Exception("JWT Token could not be created."); }


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}
