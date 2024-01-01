namespace ProductService.Models.DTOs
{
    public class ProductImageDTO
    {
        public string Image { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
    }
}
