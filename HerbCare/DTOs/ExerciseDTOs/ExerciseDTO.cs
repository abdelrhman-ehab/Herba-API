namespace HerbCare.DTOs.ExerciseDTOs
{
    public class ExerciseDTO
    {
        public int ExerciseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string Steps { get; set; }   
        public string GifUrl { get; set; }        

        public bool IsAssigned { get; set; }
    }
}
