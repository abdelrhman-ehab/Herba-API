using System.ComponentModel.DataAnnotations;

namespace HerbCare.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        public int UserId { get; set; }

        public int Quantity { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderPayment> Orders { get; set; }
    }
}
