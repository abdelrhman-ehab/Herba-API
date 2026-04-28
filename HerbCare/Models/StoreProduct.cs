namespace HerbCare.Models
{
    public class StoreProduct
    {
        public int StoreId { get; set; }
        public int ProductId { get; set; }

        // Navigation properties
        public Store Store { get; set; }
        public Product Product { get; set; }
    }
}
