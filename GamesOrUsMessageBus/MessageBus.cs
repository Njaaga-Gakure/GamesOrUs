using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace GamesOrUsMessageBus
{
    public class MessageBus : IMessageBus
    {

        public async Task PublishMessage(object message, string entityName, string connectionString)
        {
            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(entityName); 

            // convert message to json 
            var messageBody = JsonConvert.SerializeObject(message);

            // econding message
            // correlationId: unique identifier of the message
            var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
            { 
                CorrelationId = Guid.NewGuid().ToString(),
            };

            // sending the message
            await sender.SendMessageAsync(serviceBusMessage); 

            // clean up / free resourses
            await sender.DisposeAsync();    
        }
    }
}
