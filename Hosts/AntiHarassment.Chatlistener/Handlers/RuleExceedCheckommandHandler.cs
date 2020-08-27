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
    public class RuleExceedCheckommandHandler
        : IHandleMessages<RuleExceedCheckCommand>,
        IHandleMessages<UserEnteredChannelEvent>
    {
        private readonly IRuleCheckService auditActionService;
        private readonly ILogger<RuleExceedCheckommandHandler> logger;

        public RuleExceedCheckommandHandler(IRuleCheckService auditActionService, ILogger<RuleExceedCheckommandHandler> logger)
        {
            this.auditActionService = auditActionService;
            this.logger = logger;
        }

        public async Task Handle(RuleExceedCheckCommand command, IMessageHandlerContext context)
        {
            logger.LogInformation("Received Rule check command from audit for user: {arg}, from channel {arg2}", command.TwitchUsername, command.ChannelOfOrigin);

            await auditActionService.CheckBanAction(command).ConfigureAwait(false);
        }

        public async Task Handle(UserEnteredChannelEvent message, IMessageHandlerContext context)
        {
            logger.LogInformation("Received user entered channel event for user: {arg}, from channel {arg2}", message.TwitchUsername, message.ChannelOfOrigin);

            await auditActionService.CheckRulesFor(message.TwitchUsername, message.ChannelOfOrigin).ConfigureAwait(false);
        }
    }
}
