namespace HerbCare.DTOs.QuizDTOs
{
    public class QuizDTO
    {
        public int QuizId { get; set; }
        public string Title { get; set; }
        public int TotalPoints { get; set; }
        public List<QuestionDTO> Questions { get; set; }
    }
}
