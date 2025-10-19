namespace ReservationSystem.Application.DTOs
{
    public class CreateItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double PricePerHour { get; set; }
        public int ItemTypeId { get; set; }
        //public string AdminName { get; set; }
        public int AdminId { get; set; }
    }

    public class UpdateItemDto : CreateItemDto
    {
        public int Id { get; set; }
    }

    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double PricePerHour { get; set; }
        public string ItemTypeName { get; set; }
        public string CreatedByName { get; set; }
        public int CreatedById { get; set; }
        public int AdminId { get; set; }
    }

}
