using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Models.DTOs
{
    public class ProductResponseDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public List<ImageResponseDTO> ProductImages { get; set; } = new List<ImageResponseDTO>();

        public int Stock { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
    }
}
