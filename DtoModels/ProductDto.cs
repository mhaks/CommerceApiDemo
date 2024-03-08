namespace CommerceApiDemo.DtoModels
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public int AvailableQty { get; set; }
        public bool IsActive { get; set; }
    }
}
