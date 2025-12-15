using System.ComponentModel.DataAnnotations;

namespace DomainModel
{
    public class TransmissionSpec
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(64)]
        public string Type { get; set; } = string.Empty;

        public int Gears { get; set; }
    }
}
