namespace ReservationSystem.Domain.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double PricePerHour { get; set; }
        public int ItemTypeId { get; set; }
        public ItemType ItemType { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
