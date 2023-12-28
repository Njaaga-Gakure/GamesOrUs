using System.ComponentModel.DataAnnotations;

namespace CartService.Models.DTOs
{
    public class AddToCartDTO
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public int ProductQuantity { get; set; }
    }
}
