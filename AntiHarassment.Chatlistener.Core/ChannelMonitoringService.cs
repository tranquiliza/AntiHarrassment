using AntiHarassment.Chatlistener.Core.Events;
using AntiHarassment.Core;
using AntiHarassment.Core.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class ChannelMonitoringService : IChannelMonitoringService
    {
        private readonly ICompositeChatClient compositeChatClient;
        private readonly IDatetimeProvider datetimeProvider;
        private readonly IChatterRepository chatterRepository;
        private readonly IRuleCheckService ruleCheckService;
        private readonly ILogger<ChannelMonitoringService> logger;

        public ChannelMonitoringService(
            ICompositeChatClient compositeChatClient,
            IDatetimeProvider datetimeProvider,
            IChatterRepository chatterRepository,
            IRuleCheckService ruleCheckService,
            ILogger<ChannelMonitoringService> logger)
        {
            this.compositeChatClient = compositeChatClient;
            this.datetimeProvider = datetimeProvider;
            this.chatterRepository = chatterRepository;
            this.ruleCheckService = ruleCheckService;
            this.logger = logger;
        }

        public void Start()
        {
            logger.LogInformation("Starting Chat Monitoring");
            compositeChatClient.OnUserJoined += CompositeChatClient_OnUserJoined;
            logger.LogInformation("Chat Monitoring Initated");
        }

        private async Task CompositeChatClient_OnUserJoined(UserJoinedEvent e)
        {
            await chatterRepository.UpsertChatter(e.Username, datetimeProvider.UtcNow).ConfigureAwait(false);
            await ruleCheckService.CheckRulesForUserInChannel(e.Username, e.Channel).ConfigureAwait(false);
        }

        public void Dispose()
        {
            compositeChatClient.OnUserJoined -= CompositeChatClient_OnUserJoined;
        }
    }
}
