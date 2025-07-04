using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Enums
{
    public class Enum
    {
        public enum Result
        {
            Failed = 0,
            Success = 1,
            Exist = 2,
            NoDataFound = 3,
        }

        public enum AlartType
        {
            information = 0,
            warrning = 1,
            error = 2,
            success = 3
        }
        public enum AlartShow
        {
            note = 0,
            popup = 1
        }
    }
}
