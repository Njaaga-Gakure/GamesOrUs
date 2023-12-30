using System.ComponentModel.DataAnnotations;

namespace CouponService.Models.DTOs
{
    public class CouponDTO
    {
        [Required]
        public string CouponCode { get; set; } = String.Empty;
        [Required]
        public decimal CouponDiscount { get; set; }
        [Required]
        public decimal CouponMinimumAmount { get; set; }
    }
}
