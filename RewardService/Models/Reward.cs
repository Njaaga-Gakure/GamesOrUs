using System.ComponentModel.DataAnnotations.Schema;

namespace RewardService.Models
{
    public class Reward
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal OrderTotal { get; set; }

        public int RewardPoints { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
