namespace HerbCare.DTOs.IdealWeightDTOs
{
    public class IdealWeightRequestDTO
    {
        public string Gender { get; set; } // "Male" or "Female"
        public double Height { get; set; } // in cm
        public int Age { get; set; }
        public double? CurrentWeight { get; set; } // Optional current weight in kg
    }
}
