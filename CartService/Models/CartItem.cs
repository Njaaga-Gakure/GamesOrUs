using System.ComponentModel.DataAnnotations.Schema;

namespace CartService.Models
{
    public class CartItem
    {
        public Guid Id { get; set; }  
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductGenre { get; set; } = string.Empty;

        public List<CartItemImages> ProductImages { get; set; } = new List<CartItemImages>();   

        [Column(TypeName = "decimal(10,2)")]
        public decimal ProductUnitPrice { get; set; }
        public int ProductQuantity { get; set; }

        [ForeignKey("Cart")]
        public Guid CartId { get; set; }
        public Cart Cart { get; set; } = default!;
    }
}
