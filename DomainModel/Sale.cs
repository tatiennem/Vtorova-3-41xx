using DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModel
{
    public class Sale
    {
        public int Id { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.UtcNow;

        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public double DiscountPercent { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        public decimal BasePrice { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        public decimal ExtrasPrice { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        public decimal TotalPrice { get; set; }

        public string ContractPath { get; set; } = string.Empty;

        public ICollection<SaleExtraService> SaleExtraServices { get; set; } = new List<SaleExtraService>();
    }
}
