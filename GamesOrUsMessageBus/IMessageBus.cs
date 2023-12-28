namespace GamesOrUsMessageBus
{
    public interface IMessageBus
    {
        Task PublishMessage(object message, string entityName, string connectionString);
    }
}
