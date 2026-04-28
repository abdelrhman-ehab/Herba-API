namespace HerbCare.DTOs.OrderDTOs
{
    public class CreateOrderDTO
    {
        public int CartId { get; set; }
        public string Location { get; set; }
        public string PaymentMethod { get; set; }
    }
}
