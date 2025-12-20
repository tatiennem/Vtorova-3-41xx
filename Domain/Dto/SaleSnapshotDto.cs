using System;

namespace Domain.Dto
{
    public class SaleSnapshotDto
    {
        public int Id { get; set; }
        public string Car { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public double DiscountPercent { get; set; }
        public decimal TotalPrice { get; set; }
        public string ContractPath { get; set; } = string.Empty;
    }
}
