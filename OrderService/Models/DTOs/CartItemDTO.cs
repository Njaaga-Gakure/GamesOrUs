namespace OrderService.Models.DTOs
{
    public class CartItemDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductGenre { get; set; } = string.Empty;

        public string ProductImage { get; set; } = string.Empty;
        public decimal ProductUnitPrice { get; set; }
        public int ProductQuantity { get; set; }
    }
}
