

namespace Taxi.Common.Models
{
    public class OrderModel
    {
        public string DestinationAddress { get; set; } = string.Empty;
        public string StartingAddress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public CarModel Car { get; set; }
        public CategoryModel Category { get; set; }

    
    }
}
