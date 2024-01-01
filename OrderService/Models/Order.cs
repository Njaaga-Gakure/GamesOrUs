using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        [Column(TypeName = "decimal(10,2)")]
        public decimal Discount { get; set; }
        public string StripeSessionId { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string PaymentIntent { get; set; } = string.Empty;   
    }
}
