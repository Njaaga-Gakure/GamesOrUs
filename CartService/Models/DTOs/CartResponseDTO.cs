using System.ComponentModel.DataAnnotations.Schema;

namespace CartService.Models.DTOs
{
    public class CartResponseDTO
    {
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
        public decimal TotalAmount { get; set; }
    }
}
