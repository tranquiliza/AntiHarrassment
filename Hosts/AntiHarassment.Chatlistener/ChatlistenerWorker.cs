using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Core;
using Microsoft.Extensions.Hosting;

namespace AntiHarassment.Chatlistener
{
    public class ChatlistenerWorker : BackgroundService
    {
        private readonly IChatlistenerService chatlistenerService;
        private readonly ISuspensionRepository suspensionRepository;

        public ChatlistenerWorker(IChatlistenerService chatlistenerService, ISuspensionRepository suspensionRepository)
        {
            this.chatlistenerService = chatlistenerService;
            this.suspensionRepository = suspensionRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // TODO Remove in Version 1.7.0
            await suspensionRepository.MigrateSuspensionsToNewDataModel().ConfigureAwait(false);
            await chatlistenerService.ConnectAndJoinChannels().ConfigureAwait(false);
        }
    }
}
