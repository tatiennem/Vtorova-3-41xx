using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class TestDrive
    {
        public int Id { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public DateTime Slot { get; set; }

        [MaxLength(256)]
        public string Notes { get; set; } = string.Empty;
    }
}
