using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace HerbCare.Models
{
    public class Store
    {
        [Key]
        public int StoreId { get; set; }

        [Required]
        public string Name { get; set; }

        public string ContactInfo { get; set; }

        // Navigation properties
        public ICollection<StoreLocation> StoreLocations { get; set; }
        public ICollection<HerbStore> HerbStores { get; set; }
        public ICollection<UserStore> UserStores { get; set; }
        public ICollection<StoreProduct> StoreProducts { get; set; }
    }
}
