using Microsoft.EntityFrameworkCore;
using RewardService.Models;

namespace RewardService.Data
{
    public class RewardContext : DbContext
    {
        public RewardContext(DbContextOptions<RewardContext> options) : base(options) { }

        public DbSet<Reward> Rewards { get; set; }
    }
}
