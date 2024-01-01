using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartService.Models
{
    public class CartItemImages
    {
        [Key]
        public Guid Id { get; set; }
        public string Image { get; set; } = string.Empty;

        [ForeignKey("CartItem")]
        public Guid CartItemId { get; set; }
        public CartItem CartItem { get; set; } = default!;
    }
}
