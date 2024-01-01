using Azure.Messaging.ServiceBus;
using EmailsService.Models;
using EmailsService.Models;
using EmailsService.Service;
using Newtonsoft.Json;
using System.Text;

namespace EmailsService.Messaging
{
    public class GamesOrUsServiceBusConsumer : IGamesOrUsServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly string _topicName;
        private readonly string _subscriptionName;
        private readonly EmailService _emailService;
        private readonly ServiceBusProcessor _processor;
        private readonly ServiceBusProcessor _orderProcessor;

        public GamesOrUsServiceBusConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("AzureConnectionString");
            _queueName = _configuration.GetValue<string>("AzureEntities:RegisterQueue");
            _topicName = _configuration.GetValue<string>("AzureEntities:OrderTopicName");
            _subscriptionName = _configuration.GetValue<string>("AzureEntities:EmailSubcription");
            var client = new ServiceBusClient(_connectionString);
            _processor = client.CreateProcessor(_queueName);
            _orderProcessor = client.CreateProcessor(_topicName, _subscriptionName);
            _emailService = new EmailService(configuration);

        }
        public async Task Start()
        {
            _processor.ProcessMessageAsync += OnRegisterNewUser;
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync();

            _orderProcessor.ProcessMessageAsync += OnOrder;
            _orderProcessor.ProcessErrorAsync += ErrorHandler;
            await _orderProcessor.StartProcessingAsync();
        }


        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();

            await _orderProcessor.StopProcessingAsync();
            await _orderProcessor.DisposeAsync();

        }
        private async Task OnOrder(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var messageBody = Encoding.UTF8.GetString(message.Body);
            var reward = JsonConvert.DeserializeObject<RewardDTO>(messageBody);

            try
            {

                var orderMessage = new StringBuilder();
                orderMessage.Append("<div style=\"padding: 1rem; width: 80vw; margin: 0 auto;\">");
                orderMessage.Append("<img  style=\"width: 100%; height: 40%; display: block; object-fit: cover\" src=\"https://miro.medium.com/v2/resize:fit:5120/1*ZbZYU4Mfq0HBnbyHxWlMtA.png\" />");
                orderMessage.Append("<div style=\"padding: 1rem\">");
                orderMessage.Append($"<p style=\" margin-bottom: .5rem\">Dear {reward.Name},</p>");
                orderMessage.Append($"<p style=\"margin-bottom: .5rem;\">Your order of Ksh {reward.OrderTotal} has been processed successfully</p>");
                orderMessage.Append($"<p style=\"margin-bottom: .5rem;\">As of today, you've accumulated a total of {reward.RewardPoints} reward points! Your dedication and continued engagement with our platform has truly paid off.</p>");
                orderMessage.Append($"<p>Continue shopping to redeem this points for a variety of exiting prices</p>");
                orderMessage.Append($"<p>Regards,</p>");
                orderMessage.Append($"<p>GamesOrUs Team</p>");
                orderMessage.Append("</div>");
                orderMessage.Append("</div>");
                var user = new NewUserMessageDTO()
                {
                    Name = reward.Name,
                    Email = reward.Email
                };
                await _emailService.sendEmail(user, orderMessage.ToString(), "Thank you for being an invaluable part of our community.");
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;

            }
        }

        private async Task OnRegisterNewUser(ProcessMessageEventArgs arg)
        {

            var message = arg.Message;
            var messageBody = Encoding.UTF8.GetString(message.Body);
            var user = JsonConvert.DeserializeObject<NewUserMessageDTO>(messageBody);

            try
            {

                var welcomeMessage = new StringBuilder();
                welcomeMessage.Append("<div style=\"padding: 1rem; width: 80vw; margin: 0 auto;\">");
                welcomeMessage.Append("<img  style=\"width: 100%; height: 40%; display: block; object-fit: cover\" src=\"https://miro.medium.com/v2/resize:fit:5120/1*ZbZYU4Mfq0HBnbyHxWlMtA.png\" />");
                welcomeMessage.Append("<div style=\"padding: 1rem\">");
                welcomeMessage.Append($"<h1 style=\"text-align: center; margin-bottom: .5rem\">Welcome, {user.Name}</h1>");
                welcomeMessage.Append($"<p style=\"text-align: center;\">Step into an endless world of entertainment and excitement. Find your next gaming adventure here!</p>");
                welcomeMessage.Append("</div>");
                welcomeMessage.Append("</div>");
                await _emailService.sendEmail(user, welcomeMessage.ToString());
                await arg.CompleteMessageAsync(arg.Message);
            }
            catch (Exception ex)
            {
                throw;

            }
        }
    }
}
