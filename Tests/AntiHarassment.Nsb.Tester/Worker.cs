using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.NServiceBus;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AntiHarassment.Nsb.Tester
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public Worker(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var dispatcher = serviceProvider.GetService(typeof(IMessageDispatcher)) as IMessageDispatcher;

            await dispatcher.Send(new CheckDataFeedCommand()).ConfigureAwait(false);
            await dispatcher.Send(new TrainSystemCommand()).ConfigureAwait(false);

            //await dispatcher.Send(new JoinChannelCommand { ChannelName = "theodorastyles" }).ConfigureAwait(false);
        }
    }
}
