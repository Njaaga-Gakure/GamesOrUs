using Microsoft.EntityFrameworkCore;

namespace RewardService.Data
{
    public class RewardContext : DbContext
    {
        public RewardContext(DbContextOptions<RewardContext> options) : base(options) { }
    }
}
