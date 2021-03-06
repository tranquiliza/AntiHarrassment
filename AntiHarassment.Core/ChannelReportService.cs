﻿using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public class ChannelReportService : IChannelReportService
    {
        private readonly IChannelRepository channelRepository;
        private readonly ISuspensionRepository suspensionRepository;
        private readonly IChatRepository chatRepository;
        private readonly IDatetimeProvider datetimeProvider;

        public ChannelReportService(
            IChannelRepository channelRepository,
            ISuspensionRepository suspensionRepository,
            IChatRepository chatRepository,
            IDatetimeProvider datetimeProvider)
        {
            this.channelRepository = channelRepository;
            this.suspensionRepository = suspensionRepository;
            this.chatRepository = chatRepository;
            this.datetimeProvider = datetimeProvider;
        }

        public async Task<IResult<ChannelReport>> GenerateReportForChannel(string channelName, IApplicationContext context)
        {
            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);
            if (!context.HaveAccessTo(channel))
                return Result<ChannelReport>.Unauthorized();

            var suspensionsForChannel = await suspensionRepository.GetAuditedSuspensionsForChannel(channelName, datetimeProvider.UtcNow.AddYears(-1)).ConfigureAwait(false);
            if (suspensionsForChannel.Count == 0)
                return Result<ChannelReport>.NoContentFound();

            var usersForChannel = await chatRepository.GetUniqueChattersForChannel(channelName).ConfigureAwait(false);

            var suspensionsForChannelWithoutSystem = suspensionsForChannel.Where(x => x.SuspensionSource != SuspensionSource.System).ToList();
            var systemSuspensionsForChannel = suspensionsForChannel.Where(x => x.SuspensionSource == SuspensionSource.System).ToList();

            var channelReport = new ChannelReport(channelName, suspensionsForChannelWithoutSystem, systemSuspensionsForChannel, usersForChannel.Count);
            return Result<ChannelReport>.Succeeded(channelReport);
        }

        public async Task<IResult<List<UserRulesExceeded>>> GetUsersWhoExceedsRules(string channelName, IApplicationContext context)
        {
            var suspensionsForSystem = await suspensionRepository.GetSuspensions(datetimeProvider.UtcNow.AddYears(-1)).ConfigureAwait(false);
            var usersFromSuspensions = suspensionsForSystem.Select(x => x.Username);

            var channel = await channelRepository.GetChannel(channelName).ConfigureAwait(false);

            if (!context.HaveAccessTo(channel))
                return Result<List<UserRulesExceeded>>.Unauthorized();

            var allSuspensionsForChannel = await suspensionRepository.GetSuspensionsForChannel(channelName).ConfigureAwait(false);
            var allValidAuditedSuspensionForChannel = allSuspensionsForChannel.Where(x => !x.InvalidSuspension && x.Audited);

            var usersWhoExceeded = new List<UserRulesExceeded>();
            foreach (var user in usersFromSuspensions.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                // If we already have a ban
                if (allValidAuditedSuspensionForChannel.Any(x => string.Equals(x.Username, user, StringComparison.OrdinalIgnoreCase) && x.SuspensionType == SuspensionType.Ban))
                    continue;

                var suspensionsForUser = await suspensionRepository.GetSuspensionsForUser(user).ConfigureAwait(false);
                var report = new UserReport(user, suspensionsForUser);

                foreach (var rule in channel.ChannelRules.Where(x => x.ActionOnTrigger == ChannelRuleAction.Ban))
                {
                    if (report.Exceeds(rule))
                    {
                        var userRulesExceeded = usersWhoExceeded.Find(x => string.Equals(x.Username, user, StringComparison.OrdinalIgnoreCase)) ?? new UserRulesExceeded { Username = user };

                        usersWhoExceeded.Remove(userRulesExceeded);

                        userRulesExceeded.RulesBroken.Add(rule);
                        usersWhoExceeded.Add(userRulesExceeded);
                    }
                }
            }

            return Result<List<UserRulesExceeded>>.Succeeded(usersWhoExceeded);
        }
    }

    public class UserRulesExceeded
    {
        public string Username { get; set; }
        public List<ChannelRule> RulesBroken { get; set; } = new List<ChannelRule>();
    }
}