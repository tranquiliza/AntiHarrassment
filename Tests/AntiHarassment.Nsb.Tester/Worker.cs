using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.NServiceBus;
using Microsoft.Extensions.Hosting;

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

            await dispatcher.Send(new JoinChannelCommand { ChannelName = "theodorastyles" }).ConfigureAwait(false);
        }
    }
}
