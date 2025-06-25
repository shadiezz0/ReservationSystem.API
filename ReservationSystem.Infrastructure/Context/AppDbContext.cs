using Microsoft.EntityFrameworkCore;
using ReservationSystem.Domain.Entities;

namespace ReservationSystem.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ReservationItem> ReservationItems { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>()
                 .HasOne(r => r.Item)
                 .WithMany(i => i.Reservations)
                 .HasForeignKey(r => r.ItemId);
        }
    }
}
