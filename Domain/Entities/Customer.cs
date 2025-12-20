using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(32)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(128)]
        public string Email { get; set; } = string.Empty;

        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<TestDrive> TestDrives { get; set; } = new List<TestDrive>();
    }
}
