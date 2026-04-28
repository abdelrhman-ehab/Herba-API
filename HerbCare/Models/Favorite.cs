using System.ComponentModel.DataAnnotations;

namespace HerbCare.Models
{
    public class Favorite
    {
        [Key]
        public int FavId { get; set; }

        [Required]
        public int UserId { get; set; }

        public int HerbId { get; set; }

        public bool Add { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser User { get; set; }
        public Herb Herb { get; set; }
    }
}
