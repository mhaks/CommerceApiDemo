namespace CommerceApiDemo.AdminDto
{
    public class OrdersResponse
    {
        public int OrderId { get; set; }
        public string? UserName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; } = string.Empty;
        public DateTime StatusDate { get; set; }
    }
}
