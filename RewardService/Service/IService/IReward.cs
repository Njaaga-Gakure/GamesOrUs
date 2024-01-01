using RewardService.Models;

namespace RewardService.Service.IService
{
    public interface IReward
    {
        Task<string> AddReward(Reward reward);
    }
}
