namespace HerbCare.DTOs.QuizDTOs
{
    public class QuestionDTO
    {
        public int QuestionId { get; set; }
        public string? Text { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
    }
}
