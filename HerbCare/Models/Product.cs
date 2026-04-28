using System.ComponentModel.DataAnnotations;

namespace HerbCare.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Image { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        // Navigation properties
        public ICollection<StoreProduct> StoreProducts { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
