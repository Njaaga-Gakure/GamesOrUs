namespace RewardService.Messaging
{
    public interface IRewardServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
