using RewardService.Messaging;

namespace RewardService.Extensions
{
    public static class RewardServiceBusExtension
    {
        public static IRewardServiceBusConsumer rewardServiceBusConsumer { get; set; }

        public static IApplicationBuilder useAzure(this IApplicationBuilder app)
        {

            //know about the Consumer service and alos about app Lifetime

            rewardServiceBusConsumer = app.ApplicationServices.GetService<IRewardServiceBusConsumer>();

            var HostLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            HostLifetime.ApplicationStarted.Register(OnAppStart);
            HostLifetime.ApplicationStopping.Register(OnAppStopping);

            return app;

        }

        private static void OnAppStopping()
        {
           rewardServiceBusConsumer.Stop();
        }

        private static void OnAppStart()
        {
            rewardServiceBusConsumer.Start();   
        }
    }
}
