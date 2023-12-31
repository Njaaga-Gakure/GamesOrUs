namespace CartService.Models.DTOs
{
    public class CouponDTO
    {
        public Guid Id { get; set; }
        public string CouponCode { get; set; } = String.Empty;
        public decimal CouponDiscount { get; set; }
        public decimal CouponMinimumAmount { get; set; }
    }
}
