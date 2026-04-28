namespace HerbCare.Models
{
    public class UserHerb
    {
        public int UserId { get; set; }
        public int HerbId { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; }
        public Herb Herb { get; set; }
    }
}
