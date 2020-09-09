using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using AntiHarassment.Messaging.Commands;
using AntiHarassment.Messaging.Events;
using AntiHarassment.Messaging.NServiceBus;
using Microsoft.Extensions.Configuration.UserSecrets;
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

        public RuleCheckService(
            IChannelRepository channelRepository,
            ISuspensionRepository suspensionRepository,
            IMessageDispatcher messageDispatcher,
            ILogger<RuleCheckService> logger)
        {
            this.channelRepository = channelRepository;
            this.suspensionRepository = suspensionRepository;
            this.messageDispatcher = messageDispatcher;
            this.logger = logger;
        }

        public async Task CheckBanAction(RuleExceedCheckCommand command)
        {
            var suspensionsForUser = await suspensionRepository.GetSuspensionsForUser(command.TwitchUsername).ConfigureAwait(false);
            if (suspensionsForUser.Count == 0)
                return;

            var userReport = new UserReport(command.TwitchUsername, suspensionsForUser);
            var channels = await channelRepository.GetChannels().ConfigureAwait(false);
            foreach (var channel in channels.Where(x => x.ChannelRules.Count > 0))
            {
                foreach (var rule in channel.ChannelRules.Where(x => x.ActionOnTrigger == ChannelRuleAction.Ban))
                {
                    if (userReport.Exceeds(rule))
                    {
                        if (channel.SystemIsModerator && channel.ShouldListen)
                            await SendBanCommandFor(command.TwitchUsername, channel.ChannelName, rule.RuleName).ConfigureAwait(false);
                        else
                            logger.LogInformation("Channel Rule triggered ban, but channel does not have moderation / listening enabled for: {arg}", channel.ChannelName);
                    }
                }
            }
        }

        public async Task CheckRulesFor(string username, string channelName)
        {
            var channelOfOrigin = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (channelOfOrigin == null)
            {
                logger.LogWarning("We've received an event from a channel that doesnt exist?");
                return;
            }

            var suspensionsForUser = await suspensionRepository.GetSuspensionsForUser(username).ConfigureAwait(false);
            if (suspensionsForUser.Count == 0)
                return;

            var userReport = new UserReport(username, suspensionsForUser);
            foreach (var rule in channelOfOrigin.ChannelRules.Where(x => x.ActionOnTrigger == ChannelRuleAction.NotifyWebsite))
            {
                if (userReport.Exceeds(rule))
                    await SendUserExceededRuleNotifyEvent(username, channelOfOrigin.ChannelName, rule.RuleName).ConfigureAwait(false);
            }
        }

        private async Task SendBanCommandFor(string username, string channel, string ruleName)
        {
            var command = new SystemBanCommand(username, channel, $"Automated ban from rule: {ruleName}");
            await messageDispatcher.SendLocal(command).ConfigureAwait(false);
        }

        private async Task SendUserExceededRuleNotifyEvent(string username, string channel, string ruleName)
        {
            logger.LogInformation("Sending notication about {arg} on {arg2} for {arg3}", username, channel, ruleName);
            var notifyEvent = new NotifyWebsiteUserExceededRuleEvent(username, channel, ruleName);
            await messageDispatcher.Publish(notifyEvent).ConfigureAwait(false);
        }
    }
}
