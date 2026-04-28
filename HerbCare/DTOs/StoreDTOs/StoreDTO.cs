namespace HerbCare.DTOs.StoreDTOs
{
    public class StoreDTO
    {
        public int StoreId { get; set; }
        public string Name { get; set; }
        public string ContactInfo { get; set; }
        public List<string> Locations { get; set; }
    }
}
