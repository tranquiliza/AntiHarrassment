using AntiHarassment.Chatlistener.Core;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener
{
    public class ChatlistenerWorker : BackgroundService
    {
        private readonly IChatlistenerService chatlistenerService;

        public ChatlistenerWorker(IChatlistenerService chatlistenerService)
        {
            this.chatlistenerService = chatlistenerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await chatlistenerService.ConnectAndJoinChannels().ConfigureAwait(false);
        }
    }
}
