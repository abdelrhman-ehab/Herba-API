using System.ComponentModel.DataAnnotations;

namespace HerbCare.Models
{

    public class Herb
    {
        [Key]
        public int HerbId { get; set; }

        public string? Name { get; set; }

        public string? ScientificName { get; set; }
        public string? ImageLink { get; set; }
        public string Benefits { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public string SideEffects { get; set; }

        public int? CategoryId { get; set; }

        // Navigation properties
        public Category Category { get; set; }
        public ICollection<UserHerb> UserHerbs { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<HerbStore> HerbStores { get; set; }
    }

}
