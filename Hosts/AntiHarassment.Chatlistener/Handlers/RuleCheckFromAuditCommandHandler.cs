using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.Events;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class RuleCheckFromAuditCommandHandler
        : IHandleMessages<RuleCheckFromAuditCommand>,
        IHandleMessages<UserEnteredChannelEvent>
    {
        private readonly IRuleCheckService auditActionService;
        private readonly ILogger<RuleCheckFromAuditCommandHandler> logger;

        public RuleCheckFromAuditCommandHandler(IRuleCheckService auditActionService, ILogger<RuleCheckFromAuditCommandHandler> logger)
        {
            this.auditActionService = auditActionService;
            this.logger = logger;
        }

        public async Task Handle(RuleCheckFromAuditCommand command, IMessageHandlerContext context)
        {
            logger.LogInformation("Received Rule check command from audit for user: {arg}, from channel {arg2}", command.TwitchUsername, command.ChannelOfOrigin);

            await auditActionService.ReactTo(command).ConfigureAwait(false);
        }

        public async Task Handle(UserEnteredChannelEvent message, IMessageHandlerContext context)
        {
            logger.LogInformation("Received user entered channel event for user: {arg}, from channel {arg2}", message.TwitchUsername, message.ChannelOfOrigin);

            await auditActionService.CheckRulesFor(message.TwitchUsername, message.ChannelOfOrigin).ConfigureAwait(false);
        }
    }
}
