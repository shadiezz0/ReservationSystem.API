namespace ReservationSystem.Application.DTOs
{
    public class CreateReservationDto
    {
        public DateTime ReservationDate { get; set; }
        public TimeSpan StartTime { get; set; } = default(TimeSpan); //"hh:mm:ss" (e.g., "01:30:00")
        public TimeSpan EndTime { get; set; }= default(TimeSpan);
        public bool IsAvailable { get; set; } = true;
        public int ItemId { get; set; }
    }


    public class UpdateReservationDto: CreateReservationDto
    {
        public int Id { get; set; }
        public string Status { get; set; } //Pending 1 / Confirmed 2 / Cancelled 3
    }

    public class FilterReservationDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ItemId { get; set; }
    }
    public class FilterIsavilableReservayionDto: CreateReservationDto
    {

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
        public bool IsAvailable { get; set; } = true;
        public double TotalPrice { get; set; }
    }

}
