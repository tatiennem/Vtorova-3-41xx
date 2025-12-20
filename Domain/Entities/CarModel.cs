using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class CarModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(128)]
        public string BodyType { get; set; } = string.Empty;

        [MaxLength(1024)]
        public string Description { get; set; } = string.Empty;

        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}
