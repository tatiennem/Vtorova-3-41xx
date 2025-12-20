using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string Vin { get; set; } = string.Empty;

        [MaxLength(32)]
        public string StockNumber { get; set; } = string.Empty;

        [MaxLength(32)]
        public string Color { get; set; } = string.Empty;

        public int Year { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        public decimal BasePrice { get; set; }

        public bool InStock { get; set; } = true;

        public byte[] Image { get; set; } = [];

        public int ModelId { get; set; }
        public CarModel Model { get; set; } = null!;

        public int TrimLevelId { get; set; }
        public TrimLevel TrimLevel { get; set; } = null!;

        public int EngineSpecId { get; set; }
        public EngineSpec Engine { get; set; } = null!;

        public int TransmissionSpecId { get; set; }
        public TransmissionSpec Transmission { get; set; } = null!;

        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<TestDrive> TestDrives { get; set; } = new List<TestDrive>();
    }
}
