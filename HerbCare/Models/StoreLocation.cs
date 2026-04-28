using System.ComponentModel.DataAnnotations;

namespace HerbCare.Models
{
    public class StoreLocation
    {
        [Key]
        public int StoreLocationId { get; set; }

        public int StoreId { get; set; }

        [Required]
        public string Location { get; set; }

        // Navigation property
        public Store Store { get; set; }
    }
}
