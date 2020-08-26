using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Messaging.Commands;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class CheckDataFeedCommandHandler : IHandleMessages<CheckDataFeedCommand>
    {
        private readonly IChatlistenerService chatlistenerService;
        private readonly ILogger<CheckDataFeedCommandHandler> logger;

        public CheckDataFeedCommandHandler(IChatlistenerService chatlistenerService, ILogger<CheckDataFeedCommandHandler> logger)
        {
            this.chatlistenerService = chatlistenerService;
            this.logger = logger;
        }

        public async Task Handle(CheckDataFeedCommand message, IMessageHandlerContext context)
        {
            if (await chatlistenerService.CheckConnectionAndRestartIfNeeded().ConfigureAwait(false))
                logger.LogWarning("Connection problems had been detected, connection has been attempted reset");

            var options = new SendOptions();
            options.DelayDeliveryWith(TimeSpan.FromMinutes(30));
            options.RouteToThisEndpoint();

            await context.Send(new CheckDataFeedCommand(), options).ConfigureAwait(false);
            logger.LogInformation("Delayed Check Data Feed command has been sent");
        }
    }
}
