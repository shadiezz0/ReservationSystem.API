namespace ReservationSystem.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int Role { get; set; } // "Admin" or "User"
        public ICollection<Reservation> Reservations { get; set; }
    }
}
