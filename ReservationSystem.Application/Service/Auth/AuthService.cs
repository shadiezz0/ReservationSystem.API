using ReservationSystem.Application.IService.IAuth;


namespace ReservationSystem.Application.Service.Auth
{
      public class AuthService : IAuthService
      {
            private readonly IUserRepository _userRepo;
            private readonly ITokenService _tokenService;
            private readonly IUnitOfWork _uow;

            public AuthService(IUserRepository userRepo, ITokenService tokenService, IUnitOfWork uow)
            {
                  _userRepo = userRepo;
                  _tokenService = tokenService;
                  _uow = uow;
            }

            public async Task<ResponseResult> RegisterAsync(RegisterDto dto)
            {
                  var existing = await _userRepo.GetByEmailAsync(dto.Email);
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
                        RoleId = 1
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
                  var user = await _userRepo.GetByEmailAsync(dto.Email);
                  if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash ))
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

                  var token = _tokenService.GenerateToken(user);
                  return new ResponseResult
                  {
                        Result = Result.Success,
                        Data = new
                        {
                              Token = token,
                              UserId = user.Id,
                              UserName = user.Name,
                              Role = user.Role
                        },
                        Alart = new Alart
                        {
                              AlartType = AlartType.success,
                              type = AlartShow.popup,
                              MessageEn = "Login successful",
                              MessageAr = "تم تسجيل الدخول بنجاح"
                        }
                  };
            }

            public async Task<ResponseResult> ForgotPasswordAsync(ForgotPasswordDto dto)
            {
                  var user = await _userRepo.GetByEmailAsync(dto.Email);
                  if (user == null)
                        return new ResponseResult
                        {
                              Result = Result.NoDataFound,
                              Alart = new Alart
                              {
                                    AlartType = AlartType.error,
                                    type = AlartShow.popup,
                                    MessageEn = "Email not found",
                                    MessageAr = "البريد الإلكتروني غير موجود"
                              }
                        };

                  return new ResponseResult
                  {
                        Result = Result.Success,
                        Alart = new Alart
                        {
                              AlartType = AlartType.information,
                              type = AlartShow.note,
                              MessageEn = "Please contact support to reset your password",
                              MessageAr = "يرجى الاتصال بالدعم لإعادة تعيين كلمة المرور"
                        }
                  };
            }
      }


}
