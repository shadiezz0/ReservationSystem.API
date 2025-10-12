namespace ReservationSystem.Domain.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "Playground A", "Playground B"
        public string Description { get; set; }
        public double PricePerHour { get; set; }
        public int ItemTypeId { get; set; }
        public ItemType ItemType { get; set; }

        // Link item to the admin who created it
        // Nullable to handle migration scenarios where existing items don't have creators yet
        public int? CreatedById { get; set; }
        public User? CreatedBy { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
