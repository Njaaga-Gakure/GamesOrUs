using Microsoft.EntityFrameworkCore;
using RewardService.Data;
using RewardService.Models;
using RewardService.Service.IService;

namespace RewardService.Service
{
    public class RewardsService : IReward
    {

        private DbContextOptions<RewardContext> options;
        private RewardContext _context;

        public RewardsService(DbContextOptions<RewardContext> options)
        {
            this.options = options;
            _context = new RewardContext(options);
        }
        public async Task<string> AddReward(Reward reward)
        {
            await _context.Rewards.AddAsync(reward);
            await _context.SaveChangesAsync();
            return "Reward Added Succefully :(";
        }

        public async Task<Reward> GetRewardByUserEmail(string email)
        {
            return await _context.Rewards.Where(reward => reward.Email == email).FirstOrDefaultAsync();
        }

        public async Task UpdateRewardPoints(Guid id, int points)
        {
            var reward = await _context.Rewards.Where(reward => reward.Id == id).FirstOrDefaultAsync();
            reward.RewardPoints += points;
            await _context.SaveChangesAsync();
        }
    }
}
