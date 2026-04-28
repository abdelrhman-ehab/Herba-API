namespace HerbCare.DTOs.OrderDTOs
{
    public class OrderDTO
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime Date { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderItemDTO> Items { get; set; }
    }
}
