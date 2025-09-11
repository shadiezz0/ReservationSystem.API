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
            AlartShow alartShow = AlartShow.note)
        {
            return new ResponseResult
            {
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

        public static ResponseResult Success(string messageAr, string messageEn) =>
            CreateResponse(Result.Success, AlartType.success, messageAr, messageEn);

        public static ResponseResult Failed(string messageAr, string messageEn) =>
            CreateResponse(Result.Failed, AlartType.error, messageAr, messageEn);

        public static ResponseResult Warning(string messageAr, string messageEn) =>
            CreateResponse(Result.NoDataFound, AlartType.warning, messageAr, messageEn);

        public static int GetCurrentUserId()
        {
            if (_httpContextAccessor == null)
            {
                throw new InvalidOperationException("HttpContextAccessor is not initialized. Call ResponseHelper.Initialize() during startup.");
            }

            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                throw new UnauthorizedAccessException("User not authenticated or invalid user ID.");
            }

            return userId;
        }
    }
}
