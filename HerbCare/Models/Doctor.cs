using System.ComponentModel.DataAnnotations;

namespace HerbCare.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Range(0, 5)]
        public double Rating { get; set; }

        [Required]
        public string Specialty { get; set; }

        // Navigation property
        public ApplicationUser User { get; set; }
        public ICollection<Consultation> Consultations { get; set; }
    }
}