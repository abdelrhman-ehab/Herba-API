namespace HerbCare.DTOs.IdealWeightDTOs
{
    public class IdealWeightResponseDTO
    {
        public double IdealWeight { get; set; }
        public double? CurrentWeight { get; set; }
        public double? WeightDifference { get; set; }
        public string WeightStatus { get; set; }
        public string Recommendation { get; set; }
        public Dictionary<string, double> DifferentMethods { get; set; }
    }
}
