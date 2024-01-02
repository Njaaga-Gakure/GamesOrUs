using RewardService.Models;

namespace RewardService.Service.IService
{
    public interface IReward
    {
        Task<string> AddReward(Reward reward);
        Task<Reward> GetRewardByUserEmail(string email);
        Task UpdateRewardPoints(Guid id, int points);
    }
}
