namespace OrderService.Models.DTOs
{
    public class StripeRequestDTO
    {
        public string StripeSessionURL { get; set; } = default!;
        public string StripeSessionId { get; set; } = default!;
        public string ApprovedURL { get; set; } = default!;
        public string CancelURL { get; set; } = default!;   
        public Guid OrderId { get; set; }
    }
}
