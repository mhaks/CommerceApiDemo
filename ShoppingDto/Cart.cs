namespace CommerceApiDemo.ShoppingDto
{
    public class Cart
    {
        public required Customer Customer { get; set; }
        public required IList<ShoppingDto.Product> Products { get; set; }
    }
}
