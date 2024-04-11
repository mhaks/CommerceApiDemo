namespace CommerceApiDemo.AdminDto
{
    public class OrderHistoryResponse
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderStatusId { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
    }
}