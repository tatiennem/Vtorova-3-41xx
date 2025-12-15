using System;

namespace Interfaces.Dto
{
    public class CarInfoDto
    {
        public int Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Trim { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal BasePrice { get; set; }
        public string Engine { get; set; } = string.Empty;
        public string Transmission { get; set; } = string.Empty;
        public byte[] Image { get; set; } = Array.Empty<byte>();
        public bool InStock { get; set; }
    }
}
