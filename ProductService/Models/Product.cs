using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public int Stock { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }  

    }
}
