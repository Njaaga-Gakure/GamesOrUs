using Azure.Messaging.ServiceBus;
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
        private readonly EmailService _emailService;
        private readonly ServiceBusProcessor _processor;


        public GamesOrUsServiceBusConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("AzureConnectionString");
            _queueName = _configuration.GetValue<string>("AzureEntities:RegisterQueue");

            var client = new ServiceBusClient(_connectionString);
            _processor = client.CreateProcessor(_queueName);
            _emailService = new EmailService(configuration);

        }
        public async Task Start()
        {
            _processor.ProcessMessageAsync += OnRegisterNewUser;
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();

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
