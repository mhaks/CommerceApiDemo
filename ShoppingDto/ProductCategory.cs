using System.ComponentModel.DataAnnotations;

namespace CommerceApiDemo.ShoppingDto
{
    public class ProductCategory
    {
        public int Id { get; set; }        
        public string Title { get; set; } = String.Empty;
    }
}
