using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Messaging.Commands;
using NServiceBus;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class RuleExceedCheckommandHandler : IHandleMessages<RuleExceedCheckCommand>
    {
        private readonly IRuleCheckService ruleCheckService;

        public RuleExceedCheckommandHandler(IRuleCheckService ruleCheckService)
        {
            this.ruleCheckService = ruleCheckService;
        }

        public async Task Handle(RuleExceedCheckCommand command, IMessageHandlerContext context)
        {
            await ruleCheckService.CheckBanAction(command).ConfigureAwait(false);
        }
    }
}
