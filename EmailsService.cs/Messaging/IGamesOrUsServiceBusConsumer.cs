namespace EmailsService.Messaging
{
    public interface IGamesOrUsServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
