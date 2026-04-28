namespace HerbCare.DTOs.IdealWeightDTOs
{
    public class WeightCalculatorHistoryDTO
    {
        public int Id { get; set; }
        public DateTime CalculationDate { get; set; }
        public string Gender { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }
        public double IdealWeight { get; set; }
        public double? CurrentWeight { get; set; }
    }
}
