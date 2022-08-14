
using System.ComponentModel.DataAnnotations.Schema;

namespace Taxi.Model.Models
{
    [Table("Categories")]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public string Description { get; set; }=string.Empty;
        public float Price { get; set; }
        public List<Car>? Cars { get; set; }
    }
}
