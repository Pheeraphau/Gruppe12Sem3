using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class PositionModel
    {
        [Key] // This marks the property as the primary key
        public int Id { get; set; }

        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Add the missing Description property
        public string Description { get; set; }
    }
}
