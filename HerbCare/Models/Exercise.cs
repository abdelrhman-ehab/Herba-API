using System.ComponentModel.DataAnnotations;

namespace HerbCare.Models
{
    public class Exercise
    {
        [Key]
        public int ExerciseId { get; set; }
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Steps { get; set; }
        public int Duration { get; set; } // in minutes
        // GIF image link
        public string GifUrl { get; set; }
        // Navigation properties
        public ICollection<UserExercise> UserExercises { get; set; }
    }
}
