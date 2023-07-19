using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CommerceApiDem.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(140, MinimumLength = 2)]
        public string Title { get; set; } = String.Empty;

        [Required]
        [StringLength(1000, MinimumLength = 3)] 
        public string Description { get; set; } = String.Empty;

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string Brand { get; set; } = String.Empty;

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }

        [Range(0, 1000)]
        public int AvailableQty { get; set; }

        public int? ProductCategoryId { get; set; }
               
        public ProductCategory ProductCategory { get; set; } = null!;

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        public string ModelNumber { get; set; } = string.Empty;
    }
}
