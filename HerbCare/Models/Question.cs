using System.ComponentModel.DataAnnotations;

namespace HerbCare.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        public string Text { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? Correct { get; set; }

        // Navigation properties
        public ICollection<QuestionQuiz> QuestionQuizzes { get; set; }
    }
}
