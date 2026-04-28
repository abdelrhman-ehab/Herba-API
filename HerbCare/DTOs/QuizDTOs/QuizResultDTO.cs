namespace HerbCare.DTOs.QuizDTOs
{
    public class QuizResultDTO
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public int Score { get; set; }
        public int TotalPoints { get; set; }
        public int Percentage { get; set; }
        public List<AnswerResultDTO> Answers { get; set; }
    }
    public class AnswerResultDTO
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string SelectedAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public string? CorrectAnswer { get; set; }
    }
}
