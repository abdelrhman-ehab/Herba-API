using System.ComponentModel.DataAnnotations;

namespace HerbCare.Models
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        [Required]
        public string Title { get; set; }

        public int TotalPoints { get; set; }

        // Navigation properties
        public ICollection<QuestionQuiz> QuestionQuizzes { get; set; }
        public ICollection<UserQuiz> UserQuizzes { get; set; }
    }
}
