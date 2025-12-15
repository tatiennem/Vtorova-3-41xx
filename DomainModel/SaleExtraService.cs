using DomainModel;

namespace DomainModel
{
    public class SaleExtraService
    {
        public int SaleId { get; set; }
        public Sale Sale { get; set; } = null!;

        public int ExtraServiceId { get; set; }
        public ExtraService ExtraService { get; set; } = null!;

        public decimal Price { get; set; }
    }
}
