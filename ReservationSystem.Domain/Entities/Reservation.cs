using static ReservationSystem.Domain.Constants.Enums;

namespace ReservationSystem.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        public DateTime ReservationDate { get; set; } 
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;
        public double TotalPrice { get; set; }
        public Status Status { get; set; } // "Pending", "Confirmed", "Cancelled"
        public int UserId { get; set; }
        public User User { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int ItemTypeId { get; set; }// selected one from item.ItemTypes
        public ItemType ItemType { get; set; }
    }
}
// 1- time handle reservation for start and end 
// 2- status turn to enums
