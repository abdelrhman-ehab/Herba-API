using System.ComponentModel.DataAnnotations;

namespace HerbCare.Models
{
    public class IdealWeightCalculation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } // Foreign key to ApplicationUser

        public DateTime CalculationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Gender { get; set; }

        [Required]
        public double Height { get; set; } // in cm

        [Required]
        public int Age { get; set; }

        public double? CurrentWeight { get; set; } // in kg

        public double IdealWeight { get; set; } // Calculated ideal weight

        public string CalculationMethod { get; set; } // "Devine", "Robinson", "Miller", "Hamwi"

        // Navigation property
        public ApplicationUser User { get; set; }
    }
}