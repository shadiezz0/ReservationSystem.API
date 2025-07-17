namespace ReservationSystem.Application.DTOs
{
    public class ItemTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class CreateItemTypeDto
    {
        public string Name { get; set; }
    }
    public class UpdateItemTypeDto : ItemTypeDto { }
}
