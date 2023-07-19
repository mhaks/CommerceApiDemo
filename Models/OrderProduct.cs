using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommerceApiDem.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }

        public int ProductId { get; set; }

        public int OrderId { get; set; }                

        public Product Product { get; set; } = null!;
        public Order Order { get; set; } = null!;
    }
}
