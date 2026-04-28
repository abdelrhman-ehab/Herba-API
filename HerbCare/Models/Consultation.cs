using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HerbCare.Models
{
    public class Consultation
    {
        [Key]
        public int ConId { get; set; }

        [Required]
        public int UserId { get; set; }
        [Required]
        public int DoctorId { get; set; }

        public string Message { get; set; }

        public string? Reply { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("DoctorId")]
        public ApplicationUser Doctor { get; set; }
    }
}
