using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.Events;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class RuleExceedCheckommandHandler
        : IHandleMessages<RuleExceedCheckCommand>,
        IHandleMessages<UserEnteredChannelEvent>
    {
        private readonly IRuleCheckService auditActionService;

        public RuleExceedCheckommandHandler(IRuleCheckService auditActionService)
        {
            this.auditActionService = auditActionService;
        }

        public async Task Handle(RuleExceedCheckCommand command, IMessageHandlerContext context)
        {
            await auditActionService.CheckBanAction(command).ConfigureAwait(false);
        }

        public async Task Handle(UserEnteredChannelEvent message, IMessageHandlerContext context)
        {
            await auditActionService.CheckRulesFor(message.TwitchUsername, message.ChannelOfOrigin).ConfigureAwait(false);
        }
    }
}
