using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.Events;
using AntiHarassment.Messaging.NServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class RuleCheckService : IRuleCheckService
    {
        private readonly IChannelRepository channelRepository;
        private readonly ISuspensionRepository suspensionRepository;
        private readonly IMessageDispatcher messageDispatcher;
        private readonly ILogger<RuleCheckService> logger;

        public RuleCheckService(IChannelRepository channelRepository, ISuspensionRepository suspensionRepository, IMessageDispatcher messageDispatcher, ILogger<RuleCheckService> logger)
        {
            this.channelRepository = channelRepository;
            this.suspensionRepository = suspensionRepository;
            this.messageDispatcher = messageDispatcher;
            this.logger = logger;
        }

        public async Task ReactTo(RuleCheckCommand @event)
        {
            var suspensionsForUser = await suspensionRepository.GetSuspensionsForUser(@event.TwitchUsername).ConfigureAwait(false);
            if (suspensionsForUser.Count == 0)
                return;

            var userReport = new UserReport(@event.TwitchUsername, suspensionsForUser);

            var channels = await channelRepository.GetChannels().ConfigureAwait(false);
            foreach (var channel in channels.Where(x => x.ChannelRules.Count > 0
            && !string.Equals(x.ChannelName, @event.ChannelOfOrigin, StringComparison.OrdinalIgnoreCase)))
            {
                foreach (var rule in channel.ChannelRules)
                {
                    if (userReport.Exceeds(rule))
                    {
                        switch (rule.ActionOnTrigger)
                        {
                            case ChannelRuleAction.Ban:
                                if (channel.SystemIsModerator && channel.ShouldListen)
                                    await SendBanCommandFor(@event.TwitchUsername, channel.ChannelName, rule.RuleName).ConfigureAwait(false);
                                else
                                    logger.LogInformation("Channel Rule triggered ban, but channel does not have moderation / listening enabled for: {arg}", channel.ChannelName);
                                break;
                            case ChannelRuleAction.None:
                                logger.LogInformation("Audit event happened, however rule action set to none! Channel: {arg}, Rule: {arg2}", channel.ChannelName, rule.RuleId);
                                break;
                        }
                    }
                }
            }
        }

        private async Task SendBanCommandFor(string username, string channel, string ruleName)
        {
            logger.LogInformation("Sending a ban command for user: {arg1} on channel: {arg2}", username, channel);
            var command = new SystemBanCommand(username, channel, $"Automated ban from rule: {ruleName}");
            await messageDispatcher.SendLocal(command).ConfigureAwait(false);
        }
    }
}
