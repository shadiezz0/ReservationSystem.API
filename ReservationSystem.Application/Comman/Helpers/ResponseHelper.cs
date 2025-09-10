using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.Comman.Helpers
{
        public static class ResponseHelper 
    {
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
        }
}
