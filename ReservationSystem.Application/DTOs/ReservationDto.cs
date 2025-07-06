namespace ReservationSystem.Application.DTOs
{
    public class CreateReservationDto
    {
        public DateTime ReservationDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int UserId { get; set; }
        public int ItemId { get; set; }
    }


    public class UpdateReservationDto
    {
        public int Id { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } //Pending / Confirmed / Cancelled
    }

    public class ReservationDto
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string UserName { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; }
        public double TotalPrice { get; set; }
    }

}
