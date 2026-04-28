using System.ComponentModel.DataAnnotations;

namespace HerbCare.Models
{
    public class OrderPayment
    {
        [Key]
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        public int CartId { get; set; }

        public string Location { get; set; }

        public int Quantity { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string PaymentMethod { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; }
        public Cart Cart { get; set; }
    }
}
