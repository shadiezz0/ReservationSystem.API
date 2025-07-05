using static ReservationSystem.Domain.Enums.Enum;

namespace ReservationSystem.Domain.Shared
{
      public class ResponseResult
    {
        public Result Result { get; set; }
#nullable enable
        public object? DataCount { get; set; }
        public object? Data { get; set; }
        public Alart? Alart { get; set; }
        public string? Note { get; set; } // notes for front
        public int TotalCount { get; set; } // Count in Table
        public string ErrorMessageAr { get; set; }
        public string ErrorMessageEn { get; set; }
        public double Total { get; set; }
    }
    public class Alart
    {
        public AlartType AlartType { get; set; } // alart type is the type of the alart danger,info,worning and success 
        public AlartShow type { get; set; } // alart show for showing alart in notify or popup
        public string MessageAr { get; set; }
        public string MessageEn { get; set; }
    }
}
