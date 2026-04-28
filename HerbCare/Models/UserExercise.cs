namespace HerbCare.Models
{
    public class UserExercise
    {
        public int UserId { get; set; }
        public int ExerciseId { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; }
        public Exercise Exercise { get; set; }
    }
}
