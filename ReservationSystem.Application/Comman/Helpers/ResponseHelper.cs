using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ReservationSystem.Application.Comman.Helpers
{
    public static class ResponseHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Initialize(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public static ResponseResult CreateResponse(
  Result result,
  AlartType alartType,
  string messageAr,
  string messageEn,
  AlartShow alartShow = AlartShow.note,
  object data = null,
  int dataCount = 0)
        {
            return new ResponseResult
            {
                Data = data,
                DataCount = dataCount,
                Result = result,
                Alart = new Alart
                {
                    AlartType = alartType,
                    type = alartShow,
                    MessageAr = messageAr,
                    MessageEn = messageEn
                }
            };
        }

        public static ResponseResult Success(string messageAr, string messageEn, object data = null, int dataCount = 0) =>
            CreateResponse(Result.Success, AlartType.success, messageAr, messageEn, AlartShow.note, data, dataCount);

        public static ResponseResult Failed(string messageAr, string messageEn, object data = null, int dataCount = 0) =>
            CreateResponse(Result.Failed, AlartType.error, messageAr, messageEn, AlartShow.note, data, dataCount);

        public static ResponseResult Warning(string messageAr, string messageEn, object data = null, int dataCount = 0) =>
            CreateResponse(Result.NoDataFound, AlartType.warning, messageAr, messageEn, AlartShow.note, data, dataCount);

        public static  async Task<int> GetCurrentUserId()
        {
            if (_httpContextAccessor == null)
            {
                throw new InvalidOperationException("HttpContextAccessor is not initialized. Call ResponseHelper.Initialize() during startup.");
            }

            var userIdString =  _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                throw new UnauthorizedAccessException("User not authenticated or invalid user ID.");
            }

            return userId;
        }
    }
}
