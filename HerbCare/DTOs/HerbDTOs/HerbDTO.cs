namespace HerbCare.DTOs.HerbDTOs
{
    public class HerbDTO
    {
        public int HerbId { get; set; }
        public string Name { get; set; }
        public string ScientificName { get; set; }   // 👈 NEW
        public string ImageLink { get; set; }
        public string Benefits { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string SideEffects { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
