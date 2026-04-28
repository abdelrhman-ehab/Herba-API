namespace HerbCare.DTOs.ConsultationDTOs
{
    public class ConsultationDTO
    {
        public int ConId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Message { get; set; }
        public string Reply { get; set; }
        public DateTime Date { get; set; }
    }
}
