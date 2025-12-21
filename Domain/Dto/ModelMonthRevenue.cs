using System;

namespace Domain.Dto
{
    public class ModelMonthRevenue
    {
        public string Model { get; set; } = string.Empty;
        public int Sold { get; set; }
        public decimal Revenue { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal Percentage { get; set; } // Процент от общей выручки месяца
    }
}