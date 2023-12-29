using EmailsService.Messaging;

namespace EmailsService.Extensions
{
    public static class AzureServiceBusExtension
    {

        public static IGamesOrUsServiceBusConsumer serviceBusConsumer { get; set; }
        public static IApplicationBuilder useAzure(this IApplicationBuilder app)
        {
            serviceBusConsumer = app.ApplicationServices.GetService<IGamesOrUsServiceBusConsumer>();

            var hostLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostLifetime.ApplicationStarted.Register(OnAppStart);
            hostLifetime.ApplicationStopping.Register(OnAppStopping);

            return app;

        }

        private static void OnAppStopping()
        {
            serviceBusConsumer.Stop();
        }

        private static void OnAppStart()
        {
            serviceBusConsumer.Start();
        }
    }

}