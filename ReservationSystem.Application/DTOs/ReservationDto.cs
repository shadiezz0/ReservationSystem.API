namespace ReservationSystem.Application.DTOs
{
    public class ReservationDto
    {
        public int ItemId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ReservedBy { get; set; }

    }
}
