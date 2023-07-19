using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommerceApiDem.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = null!;
       
        public ICollection<OrderProduct> OrderProducts { get; set; } = null!;

        public ICollection<OrderHistory> OrderHistory { get; set; } = null!;

        [Display(Name ="Sub Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Subtotal { 
            get
            {
                decimal subtotal = 0;
                if (OrderProducts != null) 
                {
                    foreach (var item in OrderProducts)
                    {
                        subtotal += item.Price * item.Quantity;

                    }
                }
                
                return subtotal;
            }                
        }

        [Display(Name = "Tax")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Tax
        {
            get
            {
                return Subtotal * User.StateLocation.TaxRate;
            }
        }

        [Display(Name = "Total Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalPrice
        {
            get
            {
                return Subtotal + Tax;
            }
        }


       
    }
}