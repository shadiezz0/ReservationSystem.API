namespace ReservationSystem.Domain.Entities
{
    public class ReservationItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // Playground, PlayStation, etc.
        public string? Description { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
