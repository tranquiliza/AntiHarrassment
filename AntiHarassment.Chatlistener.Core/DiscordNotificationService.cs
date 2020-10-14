using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class DiscordNotificationService : IDiscordNotificationService
    {
        private readonly IUserRepository userRepository;
        private readonly IChannelRepository channelRepository;
        private readonly IDiscordMessageClient discordMessageClient;
        private readonly ISuspensionRepository suspensionRepository;
        private readonly ILogger<DiscordNotificationService> logger;

        public DiscordNotificationService(
            IUserRepository userRepository,
            IChannelRepository channelRepository,
            IDiscordMessageClient discordMessageClient,
            ISuspensionRepository suspensionRepository,
            ILogger<DiscordNotificationService> logger)
        {
            this.userRepository = userRepository;
            this.channelRepository = channelRepository;
            this.discordMessageClient = discordMessageClient;
            this.suspensionRepository = suspensionRepository;
            this.logger = logger;
        }

        public async Task SendNotification(string username, string channelOfOrigin, Guid ruleId)
        {
            var channel = await channelRepository.GetChannel(channelOfOrigin).ConfigureAwait(false);
            var ruleThatWasBroken = channel.ChannelRules.FirstOrDefault(x => x.RuleId == ruleId);

            var suspensionsForUser = await suspensionRepository.GetSuspensionsForUser(username).ConfigureAwait(false);
            if (UserHasActiveBanInTheChannel())
            {
                logger.LogInformation($"{username} already has active bans in {channelOfOrigin}, no need to send notification to discord");
                return;
            }

            var ownerOfChannel = await userRepository.GetByTwitchUsername(channel.ChannelName).ConfigureAwait(false);
            if (ownerOfChannel?.DiscordEnabled == true)
            {
                logger.LogInformation($"Sent discord notification to {ownerOfChannel.TwitchUsername} (Owner of channel)");

                await discordMessageClient.SendDmToUser(ownerOfChannel.DiscordUserId, $"{username}, has been spotted in your channel, they have exceeded the rule: {ruleThatWasBroken.RuleName}").ConfigureAwait(false);
            }

            foreach (var mod in channel.Moderators)
            {
                var user = await userRepository.GetByTwitchUsername(mod).ConfigureAwait(false);
                if (user?.DiscordEnabled == true)
                {
                    logger.LogInformation($"Sent discord notification to {user.TwitchUsername}");

                    await discordMessageClient.SendDmToUser(user.DiscordUserId, $"{username}, has been spotted in {channel.ChannelName}, they have exceeded the rule: {ruleThatWasBroken.RuleName}").ConfigureAwait(false);
                }
            }

            bool UserHasActiveBanInTheChannel()
            {
                return suspensionsForUser.Any(x =>
                    string.Equals(x.ChannelOfOrigin, channelOfOrigin, StringComparison.OrdinalIgnoreCase)
                    && x.SuspensionType == SuspensionType.Ban
                    && x.Audited
                    && !x.InvalidSuspension);
            }
        }
    }
}
