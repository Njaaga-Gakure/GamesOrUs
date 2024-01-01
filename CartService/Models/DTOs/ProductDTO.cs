using System.ComponentModel.DataAnnotations.Schema;

namespace CartService.Models.DTOs
{
    public class ProductDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;
        public List<ProductImageDTO> ProductImages { get; set; } = new List<ProductImageDTO>();

        public int Stock { get; set; }

        public decimal Price { get; set; }
    }
}
