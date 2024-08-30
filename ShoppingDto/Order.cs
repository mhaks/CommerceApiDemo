using CommerceApiDem.Models;


namespace CommerceApiDemo.ShoppingDto
{
    public class Order
    {
        public int Id { get; set; }
        public required Customer Customer { get; set; }
        public required ICollection<Product> Products { get; set; }
        public required ICollection<History> History { get; set; }
        
        public decimal Subtotal
        {
            get
            {
                decimal subtotal = 0;
                if (Products != null)
                {
                    foreach (var item in Products)
                    {
                        subtotal += item.Price * item.Quantity;

                    }
                }

                return subtotal;
            }
        }
        public decimal Tax
        {
            get
            {
                return Subtotal * Customer.TaxRate;
            }
        }
        public decimal TotalPrice
        {
            get
            {
                return Subtotal + Tax;
            }
        }
    }
}
