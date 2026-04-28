namespace HerbCare.Models
{
    public class UserStore
    {
        public int UserId { get; set; }
        public int StoreId { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; }
        public Store Store { get; set; }
    }
}
