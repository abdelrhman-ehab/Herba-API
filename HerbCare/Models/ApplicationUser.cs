using HerbCare.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HerbCare.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public string Gender { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        // Add UserType to Identity User
        public UserType UserType { get; set; }

        // Doctor-specific properties (nullable for regular users)
        public string? Phone { get; set; }
        public string? Specialty { get; set; }
        public double Rating { get; set; }

        // Navigation properties
        public ICollection<UserExercise> UserExercises { get; set; }
        public ICollection<UserQuiz> UserQuizzes { get; set; }
        public ICollection<UserHerb> UserHerbs { get; set; }
        public ICollection<UserStore> UserStores { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        // For consultations - User can create consultations
        public ICollection<Consultation> UserConsultations { get; set; }

        // For consultations - Doctor can receive consultations
        public ICollection<Consultation> DoctorConsultations { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<OrderPayment> Orders { get; set; }
        // Add to existing navigation properties in ApplicationUser class
        public ICollection<IdealWeightCalculation> IdealWeightCalculations { get; set; }
    }
}