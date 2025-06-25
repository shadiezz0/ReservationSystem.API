using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Entities
{
    public class ItemType
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "Playground", "PlayStation"
        public ICollection<Item> Items { get; set; }
    }
}
