using AutoMapper;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using RewardService.Models;
using RewardService.Models.DTOs;
using RewardService.Service;
using RewardService.Service.IService;
using System.Text;

namespace RewardService.Messaging
{
    public class RewardServiceBusConsumer : IRewardServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _topic;
        private readonly string _subscription;
        private readonly ServiceBusProcessor _rewardsProcessor;
        private readonly RewardsService _rewardService;
        private readonly IMapper _mapper;

        public RewardServiceBusConsumer(IConfiguration configuration, RewardsService rewardService, IMapper mapper)
        {
            _mapper = mapper;
            _rewardService = rewardService;
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("AzureConnectionString");
            _topic = _configuration.GetValue<string>("AzureEntities:OrderTopicName");
            _subscription = _configuration.GetValue<string>("AzureEntities:RewardSubcription");
            var client = new ServiceBusClient(_connectionString);
            _rewardsProcessor = client.CreateProcessor(_topic, _subscription);
        }

        public async Task Start()
        {
            _rewardsProcessor.ProcessMessageAsync += OnReward;
            _rewardsProcessor.ProcessErrorAsync += ErrorHandler; 

            await _rewardsProcessor.StartProcessingAsync(); 

        }
        public async Task Stop()
        {
            await _rewardsProcessor.StopProcessingAsync();
            await _rewardsProcessor.DisposeAsync();   
        }
        private async Task OnReward(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var messageBody = Encoding.UTF8.GetString(message.Body);
            var reward = JsonConvert.DeserializeObject<RewardDTO>(messageBody);

            try
            {
                // check if a user already has a reward by email (email is a unique field)
                var existingReward = await _rewardService.GetRewardByUserEmail(reward.Email);
                if (existingReward != null)
                {
                    await _rewardService.UpdateRewardPoints(existingReward.Id, reward.RewardPoints);
                }
                else
                { 
                    var newReward = _mapper.Map<Reward>(reward);
                    await _rewardService.AddReward(newReward);
                    await args.CompleteMessageAsync(args.Message);
                }

            
            }
            catch (Exception ex) 
            {
                throw;
            }
                
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            return Task.CompletedTask;
        }


    }
}
