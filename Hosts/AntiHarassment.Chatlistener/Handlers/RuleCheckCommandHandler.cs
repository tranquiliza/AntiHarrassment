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
    public class RuleCheckCommandHandler : IHandleMessages<RuleCheckCommand>
    {
        private readonly IRuleCheckService auditActionService;
        private readonly ILogger<RuleCheckCommandHandler> logger;

        public RuleCheckCommandHandler(IRuleCheckService auditActionService, ILogger<RuleCheckCommandHandler> logger)
        {
            this.auditActionService = auditActionService;
            this.logger = logger;
        }

        public async Task Handle(RuleCheckCommand command, IMessageHandlerContext context)
        {
            logger.LogInformation("Received Rule check command for user: {arg}, from channel {arg2}", command.TwitchUsername, command.ChannelOfOrigin);

            await auditActionService.ReactTo(command).ConfigureAwait(false);
        }
    }
}
