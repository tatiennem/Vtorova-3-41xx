namespace Domain.Dto
{
    public class PriceBreakdown
    {
        public decimal BasePrice { get; set; }
        public decimal ExtrasPrice { get; set; }
        public double DiscountPercent { get; set; }
        public decimal Total => (BasePrice - BasePrice * (decimal)(DiscountPercent / 100.0)) + ExtrasPrice;
    }
}
