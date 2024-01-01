using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Models
{
    public class ProductImage
    {
        [Key]
        public Guid Id { get; set; }    
        public string Image { get; set; } = string.Empty;

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        public Product Product { get; set; } = default!;    
    }
}
