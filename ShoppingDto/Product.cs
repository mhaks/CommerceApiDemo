namespace CommerceApiDemo.ShoppingDto
{
    public class Product
    {
        public required int Id { get; set; }
        public required string Title { get; set; } 
        public required string Description { get; set; }
        public required string Brand { get; set; } 
        public required string Model { get; set; } 
        public required string Category { get; set; }
        public required decimal Price { get; set; }
        public required int AvailableQty { get; set; }
        public required int Quantity { get; set; }
    }
}
