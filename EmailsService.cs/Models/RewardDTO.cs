namespace EmailsService.Models
{
    public class RewardDTO
    {
        public Guid OrderId { get; set; }
        public decimal OrderTotal { get; set; }

        public int RewardPoints { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
