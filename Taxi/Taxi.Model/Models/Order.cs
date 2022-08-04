

using System.ComponentModel.DataAnnotations.Schema;

namespace Taxi.Model.Models
{
    [Table("Orders")]
    public class Order
    {
        public int Id { get; set; }
        public string DestinationAddress { get; set; } = string.Empty;
        public string StartingAddress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public Category? Category{get;set;}
        public int? CarId { get; set; }
        public Car? Car { get; set; }
    }
}
