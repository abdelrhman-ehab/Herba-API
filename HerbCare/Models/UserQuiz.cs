namespace HerbCare.Models
{
    public class UserQuiz
    {
        public int UserId { get; set; }
        public int QuizId { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; }
        public Quiz Quiz { get; set; }
    }
}
