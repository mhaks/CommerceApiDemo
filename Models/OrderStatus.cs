using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CommerceApiDem.Models;

namespace CommerceApiDem.Models
{
    
    public class OrderStatus
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = String.Empty;
    }

    public enum OrderState
    {
         Cart = 1,
         Processing = 2,
         Shipped = 3,
         Delivered = 4,
         Returned = 5,                      
    }
}
