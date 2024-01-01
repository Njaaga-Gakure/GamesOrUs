using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Models.DTOs
{
    public class CartDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
        public decimal TotalAmount { get; set; }
    }
}
