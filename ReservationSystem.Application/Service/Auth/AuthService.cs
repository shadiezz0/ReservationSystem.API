using ReservationSystem.Application.DTOs;
using ReservationSystem.Application.IService.IAuth;
using ReservationSystem.Domain.Entities;
using ReservationSystem.Domain.Interfaces;
using ReservationSystem.Domain.Shared;
using System.Security.Claims;
using System.Text;
using static ReservationSystem.Domain.Constants.Enums;

namespace ReservationSystem.Application.Service.Auth
{
    public class AuthService : IAuthService
      {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IConfiguration _config;

            public AuthService(IUnitOfWork unitOfWork, IConfiguration config)
            {
                  _unitOfWork = unitOfWork;
                  _config = config;
            }

            public async Task<ResponseResult> RegisterAsync(RegisterDto dto)
            {
                  var exists = (await _unitOfWork.Repository<User>().GetAllAsync())
                               .Any(u => u.Email == dto.Email);

                  if (exists)
                  {
                        ResponseResult  res= new ResponseResult
                        {
                             Result =Result.Exist,
                              Alart = new Alart
                              {
                                    AlartType = AlartType.warrning,
                                    type = AlartShow.popup,
                                    MessageAr = "البريد الإلكتروني موجود بالفعل",
                                    MessageEn = "Email already exists"
                              }

                        };
                  }

                  var user = new User
                  {
                        FullName = dto.FullName,
                        Email = dto.Email,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                        Role = "User"
                  };

                  await _unitOfWork.Repository<User>().AddAsync(user);
                  await _unitOfWork.SaveAsync();

                  result.Note = "Registration successful";
                  return result;
            }

            public async Task<ResponseResult> LoginAsync(LoginDto dto)
            {
                  var result = new ResponseResult();
                  var user = (await _unitOfWork.Repository<User>().GetAllAsync())
                             .FirstOrDefault(u => u.Email == dto.Email);

                  if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                  {
                        result.Result.IsSuccess = false;
                        result.Result.Message = "Invalid credentials";
                        return result;
                  }

                  var token = GenerateJwtToken(user);
                  result.Data = token;
                  return result;
            }

            public async Task<ResponseResult> ForgotPasswordAsync(ForgotPasswordDto dto)
            {
                  var result = new ResponseResult();
                  var user = (await _unitOfWork.Repository<User>().GetAllAsync())
                             .FirstOrDefault(u => u.Email == dto.Email);

                  if (user == null)
                  {
                        result.Result.IsSuccess = false;
                        result.Result.Message = "Email not found";
                        return result;
                  }

                  // simulate reset
                  result.Note = "Reset link sent (simulated)";
                  return result;
            }

            private string GenerateJwtToken(User user)
            {
                  var claims = new[]
                  {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

                  var key = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
                  var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                  var token = new JwtSecurityToken(
                      issuer: _config["Jwt:Issuer"],
                      audience: _config["Jwt:Audience"],
                      claims: claims,
                      expires: DateTime.UtcNow.AddHours(2),
                      signingCredentials: creds
                  );

                  return new JwtSecurityTokenHandler().WriteToken(token);
            }
      }

}
