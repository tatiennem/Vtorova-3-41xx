using System.ComponentModel.DataAnnotations;

namespace DomainModel
{
    public class EngineSpec
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(32)]
        public string FuelType { get; set; } = string.Empty;

        public double Volume { get; set; }

        public int Power { get; set; }
    }
}
