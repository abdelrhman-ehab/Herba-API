namespace HerbCare.Models
{
    public class QuestionQuiz
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }

        // Navigation properties
        public Question Question { get; set; }
        public Quiz Quiz { get; set; }
    }
}
