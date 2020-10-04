using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Core;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener
{
    public class ChatlistenerWorker : BackgroundService
    {
        private readonly IChatlistenerService chatlistenerService;
        private readonly IDiscordMessageClient discordMessageClient;

        public ChatlistenerWorker(IChatlistenerService chatlistenerService, IDiscordMessageClient discordMessageClient)
        {
            this.chatlistenerService = chatlistenerService;
            this.discordMessageClient = discordMessageClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await chatlistenerService.ConnectAndJoinChannels().ConfigureAwait(false);

            await discordMessageClient.Initialize().ConfigureAwait(false);

            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            await discordMessageClient.SendMessageToPrometheus("Chatlisterner started, Discord Client Started").ConfigureAwait(false);
            await discordMessageClient.SendMessageToPrometheus(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") + " environment ready to go!").ConfigureAwait(false);
        }

        public async override Task StopAsync(CancellationToken cancellationToken)
        {
            await discordMessageClient.DisposeAsync();
        }
    }
}
