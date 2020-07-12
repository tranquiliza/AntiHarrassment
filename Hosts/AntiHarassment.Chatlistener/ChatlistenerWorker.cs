using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AntiHarassment.Chatlistener.Core;
using Microsoft.Extensions.Hosting;

namespace AntiHarassment.Chatlistener
{
    public class ChatlistenerWorker : BackgroundService
    {
        private readonly IChatlistenerService chatlistenerService;

        public ChatlistenerWorker(IChatlistenerService chatlistenerService)
        {
            this.chatlistenerService = chatlistenerService;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            // TODO Possible stop the chat listener (Disconnect)
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await chatlistenerService.ConnectAndJoinChannels().ConfigureAwait(false);
        }
    }
}
