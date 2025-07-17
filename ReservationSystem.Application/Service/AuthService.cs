using ReservationSystem.Application.IService;
using System.Security.Claims;


namespace ReservationSystem.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Role> _roleRepo;
        private readonly ITokenService _tokenService;

        public AuthService(IUnitOfWork uow, ITokenService tokenService)
        {
            _uow = uow;
            _userRepo = uow.Repository<User>();
            _roleRepo = uow.Repository<Role>();
            _tokenService = tokenService;
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
                RoleId = dto.RoleId,
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


            //var token = _tokenService.GenerateToken(user);
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
            if (userEmail == null)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageEn = "User email not found in token",
                        MessageAr = "البريد الإلكتروني للمستخدم غير موجود في الرمز"
                    }
                };

            var user = await _userRepo.FindOneAsync(u => u.Email == userEmail);

            if (user == null)
                return new ResponseResult
                {
                    Result = Result.NoDataFound,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageEn = "User not found",
                        MessageAr = "المستخدم غير موجود"
                    }
                };

            var roleName = await _roleRepo.GetByIdAsync(user.RoleId);
            if (roleName == null)
                return new ResponseResult
                {
                    Result = Result.NoDataFound,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageEn = "Role not found for the user",
                        MessageAr = "الدور غير موجود للمستخدم"
                    }
                };

            var accessToken = _tokenService.GenerateAccessToken(user, roleName.Name);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return new ResponseResult
            {
                Result = Result.Success,
                Data = new TokenDto { AccessToken = accessToken, RefreshToken = refreshToken },
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.popup,
                    MessageEn = "Tokens refreshed successfully",
                    MessageAr = "تم تحديث الرموز بن"
                }
            };

        }






    }


}
