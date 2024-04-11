namespace CommerceApiDemo.AdminDto
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public CustomerResponse Customer { get; set; } = null!;
        public ICollection<ProductResponse> Products { get; set; } = null!;
        public ICollection<OrderHistoryResponse> History { get; set; } = null!;
    }
}
