using ReservationSystem.Domain.Entities;

namespace ReservationSystem.Application.DTOs
{
      public class ReservationDto
      {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int ItemId { get; set; }
            public DateTime ReservationDate { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public decimal TotalPrice { get; set; }
            public string Status { get; set; }
       


    }
}
