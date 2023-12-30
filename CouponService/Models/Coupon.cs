using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CouponService.Models
{
    public class Coupon
    {
        [Key]
        public Guid Id { get; set; }
        public string CouponCode { get; set; } = String.Empty;
        [Column(TypeName = "decimal(10,2)")]
        public decimal CouponDiscount { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal CouponMinimumAmount { get; set; }
    }
}
