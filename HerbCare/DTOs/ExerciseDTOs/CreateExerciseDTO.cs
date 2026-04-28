namespace HerbCare.DTOs.ExerciseDTOs
{
    public class CreateExerciseDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public int Duration { get; set; }
        public string AssignmentExercise { get; set; }

        public string Steps { get; set; }  
        public string GifUrl { get; set; }      
    }
}
