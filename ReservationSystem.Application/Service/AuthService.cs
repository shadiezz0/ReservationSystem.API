using Microsoft.Extensions.Options;
using System.Security.Claims;


namespace ReservationSystem.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Permission> _permRepo;
        private readonly IGenericRepository<RolePermission> _rolePermRepo;
        private readonly IGenericRepository<Role> _roleRepo;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepo;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUnitOfWork uow, ITokenService tokenService, IOptions<JwtSettings> jwtOptions)
        {
            _uow = uow;
            _userRepo = uow.Repository<User>();
            _roleRepo = uow.Repository<Role>();
            _permRepo = uow.Repository<Permission>();
            _rolePermRepo = uow.Repository<RolePermission>();
            _refreshTokenRepo = uow.Repository<RefreshToken>();
            _tokenService = tokenService;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<ResponseResult> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userRepo.FindOneAsync(e => e.Email == dto.Email);
            if (existing != null)
                return new ResponseResult
                {
                    Result = Result.Exist,

                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageEn = "Email already exists",
                        MessageAr = "البريد الإلكتروني موجود بالفعل"
                    }
                };

            var user = new User
            {
                Name = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = 2 //user role,
            };

            await _userRepo.AddAsync(user);
            await _uow.SaveAsync();

            return new ResponseResult
            {
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.popup,
                    MessageEn = "Registration successful",
                    MessageAr = "تم التسجيل بنجاح"
                },
            };
        }

        public async Task<ResponseResult> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.FindOneAsync(e => e.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageEn = "Invalid email or password",
                        MessageAr = "البريد الإلكتروني أو كلمة المرور غير صحيحة"
                    }
                };

            var role = await _roleRepo.GetByIdAsync(user.RoleId);
            var access = _tokenService.GenerateAccessToken(user, role.Name);
            var refresh = _tokenService.GenerateRefreshToken();

            var refreshEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refresh,
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                IsRevoked = false,
                IsUsed = false
            };

            await _refreshTokenRepo.AddAsync(refreshEntity);
            await _uow.SaveAsync();

            return new ResponseResult
            {
                Result = Result.Success,
                Data = new TokenDto { AccessToken = access, RefreshToken = refresh },
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.popup,
                    MessageEn = "Login successful",
                    MessageAr = "تم تسجيل الدخول بنجاح"
                }
            };
        }

        public async Task<ResponseResult> RefreshTokenAsync(TokenDto dto)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
            if (principal == null)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageEn = "Invalid access token",
                        MessageAr = "رمز الوصول غير صالح"
                    }
                };

            var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;

            var user = await _userRepo.FindOneAsync(u => u.Email == userEmail);

            var oldToken = await _refreshTokenRepo
                .FindOneAsync(rt => rt.UserId == user.Id && rt.Token == dto.RefreshToken && !rt.IsRevoked && !rt.IsUsed);

            if (oldToken == null || oldToken.IsRevoked || oldToken.IsUsed || oldToken.Expires < DateTime.UtcNow)
                return new ResponseResult
                {
                    Result = Result.NoDataFound,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageEn = "Refresh token not found or invalid",
                        MessageAr = "رمز التحديث غير موجود أو غير صالح"
                    }
                };

            oldToken.IsRevoked = true;
            oldToken.IsUsed = true;

            var roleName = await _roleRepo.GetByIdAsync(user.RoleId);

            var newAccess = _tokenService.GenerateAccessToken(user, roleName.Name);
            var newRefresh = _tokenService.GenerateRefreshToken();

            var newToken = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefresh,
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                IsRevoked = false,
                IsUsed = false
            };

            _refreshTokenRepo.Update(oldToken);
            await _refreshTokenRepo.AddAsync(newToken);
            await _uow.SaveAsync();


            return new ResponseResult
            {
                Result = Result.Success,
                Data = new TokenDto { AccessToken = newAccess, RefreshToken = newRefresh },
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.popup,
                    MessageEn = "Token refreshed successfully",
                    MessageAr = "تم تحديث الرمز بنجاح"
                }
            };

        }

     
    }


}
