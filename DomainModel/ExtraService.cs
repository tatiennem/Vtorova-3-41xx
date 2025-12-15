using DomainModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModel
{
    public class ExtraService
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(96)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(256)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "numeric(18,2)")]
        public decimal Price { get; set; }

        public ICollection<SaleExtraService> SaleExtraServices { get; set; } = new List<SaleExtraService>();
    }
}
