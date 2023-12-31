using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CartService.Models.DTOs;

namespace CartService.Models
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CouponCode { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Discount { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
    }
}
