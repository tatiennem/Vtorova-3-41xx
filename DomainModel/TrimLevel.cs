using DomainModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DomainModel
{
    public class TrimLevel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(256)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Price delta compared to base model price.
        /// </summary>
        [Range(-500000, 500000)]
        public decimal PriceDelta { get; set; }

        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}
