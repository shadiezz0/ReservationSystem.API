namespace ReservationSystem.Domain.Entities
{
    public class ItemType
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "Service of Playground", "PlayStation"

        // ✅ Many-to-many back relation
        public ICollection<Item> Items { get; set; } = new List<Item>();

    }
}
