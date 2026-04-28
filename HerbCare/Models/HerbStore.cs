namespace HerbCare.Models
{
    public class HerbStore
    {
        public int HerbId { get; set; }
        public int StoreId { get; set; }

        // Navigation properties
        public Herb Herb { get; set; }
        public Store Store { get; set; }
    }
}
