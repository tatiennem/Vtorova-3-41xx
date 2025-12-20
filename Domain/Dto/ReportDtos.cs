using System;

namespace Domain.Dto
{
    public class MonthlySalesReport
    {
        public string Period { get; set; } = string.Empty;
        public int CarsSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class ModelSalesReport
    {
        public string Model { get; set; } = string.Empty;
        public int Sold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TestDriveScheduleItem
    {
        public DateTime Slot { get; set; }
        public string Car { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
