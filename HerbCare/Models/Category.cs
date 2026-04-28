using System.ComponentModel.DataAnnotations;

namespace HerbCare.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        // Navigation properties
        public ICollection<Herb> Herbs { get; set; }
    }
}
