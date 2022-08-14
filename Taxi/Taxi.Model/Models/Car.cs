using System.ComponentModel.DataAnnotations.Schema;
namespace Taxi.Model.Models
{
    [Table ("Cars")]
    public class Car
    {
        public int Id { get; set; }
        public string Model { get; set; }=string.Empty;
        public string Number { get; set; }=string.Empty;
        public int? DriverId { get; set; }
        public Driver? Driver { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<Order>? Orders { get; set; }
    }
}
