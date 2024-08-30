namespace CommerceApiDemo.ShoppingDto
{
    public class History
    {
        public required DateTime OrderDate { get; set; }
        public required int StatusId { get; set; }
        public required string Status { get; set; }
    }
}
