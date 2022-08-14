
using System.ComponentModel.DataAnnotations.Schema;

namespace Taxi.Model.Models
{
    [Table("Drivers")]
    public class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public float DrivingExperience { get; set; }
        public List<Car>? Cars { get; set; }
    }
}
