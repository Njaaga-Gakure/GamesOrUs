using RewardService.Data;
using RewardService.Models;
using RewardService.Service.IService;

namespace RewardService.Service
{
    public class RewardsService : IReward
    {

        private readonly RewardContext _context;

        public RewardsService(RewardContext context)
        {
              _context = context;   
        }
        public async Task<string> AddReward(Reward reward)
        {
            await _context.Rewards.AddAsync(reward);
            await _context.SaveChangesAsync();
            return "Reward Added Succefully :(";
        }
    }
}
