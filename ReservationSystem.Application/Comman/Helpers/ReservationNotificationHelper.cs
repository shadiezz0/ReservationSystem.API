using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.Comman.Helpers
{
    public static class ReservationNotificationHelper
    {
        public static ( string msgAr,  string msgEn)
            GetMessage(Status status)
        {
            return status switch
            {
                Status.Pending => (   
                    "تم إنشاء الحجز وهو قيد المراجعة.",
                    "Your reservation is pending approval."
                ),

                Status.Confirmed => (                  
                    "تمت الموافقة على حجزك بنجاح.",
                    "Your reservation has been confirmed."
                ),

                Status.Cancelled => (
                    "تم إلغاء الحجز. يرجى مراجعة التفاصيل.",
                    "Your reservation has been cancelled."
                ),

                _ => (
                    "تم تحديث حالة الحجز.",
                    "Reservation status updated."
                )
            };
        }
    }
}
