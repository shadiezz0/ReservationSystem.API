namespace ReservationSystem.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ReservedBy { get; set; }

        public int ItemId { get; set; }
        public ReservationItem Item { get; set; }
    }
}
