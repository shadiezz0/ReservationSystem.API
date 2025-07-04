namespace ReservationSystem.Application.DTOs
{
      public class ItemDto
      {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ItemTypeId { get; set; }
            public string Description { get; set; }
            public decimal PricePerHour { get; set; }
            public bool IsAvailable { get; set; }
      }
}
