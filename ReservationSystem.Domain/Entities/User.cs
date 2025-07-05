namespace ReservationSystem.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Role Relationship (one-to-many)
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}