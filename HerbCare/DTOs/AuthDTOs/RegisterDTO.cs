namespace HerbCare.DTOs.AuthDTOs
{
    public class RegisterDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }

        // New property to specify user type
        public UserType UserType { get; set; }

        // Doctor-specific fields (only required if UserType is Doctor)
        public string? Phone { get; set; }
        public string? Specialty { get; set; }
    }
    public enum UserType
    {
        User,
        Doctor
    }
}
