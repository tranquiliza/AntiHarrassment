using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Core;
using AntiHarassment.Messaging.Commands;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class CheckDataFeedCommandHandler : IHandleMessages<CheckDataFeedCommand>
    {
        private readonly IChatlistenerService chatlistenerService;
        private readonly ILogger<CheckDataFeedCommandHandler> logger;
        private readonly IDiscordMessageClient discordMessageClient;

        public CheckDataFeedCommandHandler(IChatlistenerService chatlistenerService, ILogger<CheckDataFeedCommandHandler> logger, IDiscordMessageClient discordMessageClient)
        {
            this.chatlistenerService = chatlistenerService;
            this.logger = logger;
            this.discordMessageClient = discordMessageClient;
        }

        public async Task Handle(CheckDataFeedCommand message, IMessageHandlerContext context)
        {
            if (await chatlistenerService.CheckConnectionAndRestartIfNeeded().ConfigureAwait(false))
            {
                const string logMessage = "Connection problems had been detected, connection has been attempted reset";
                logger.LogWarning(logMessage);
                await discordMessageClient.SendMessageToPrometheus(logMessage).ConfigureAwait(false);
            }

            var options = new SendOptions();
            options.DelayDeliveryWith(TimeSpan.FromMinutes(30));
            options.RouteToThisEndpoint();

            await context.Send(new CheckDataFeedCommand(), options).ConfigureAwait(false);
            logger.LogInformation("Delayed Check Data Feed command has been sent");
        }
    }
}
