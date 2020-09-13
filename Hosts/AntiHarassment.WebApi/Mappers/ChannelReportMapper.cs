using AntiHarassment.Contract;
using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace AntiHarassment.WebApi.Mappers
{
    public static class ChannelReportMapper
    {
        public static List<UserRulesExceededModel> Map(this List<UserRulesExceeded> input)
        {
            return input.Select(Map).ToList();
        }

        public static UserRulesExceededModel Map(this UserRulesExceeded input)
        {
            return new UserRulesExceededModel
            {
                RulesBroken = input.RulesBroken.Select(x => x.Map()).ToList(),
                Username = input.Username
            };
        }

        public static ChannelReportModel Map(this ChannelReport channelReport)
        {
            return new ChannelReportModel
            {
                ChannelName = channelReport.ChannelName,
                SuspendedUsers = channelReport.SuspendedUsers,
                BannedUsers = channelReport.BannedUsers,
                TimedoutUsers = channelReport.TimedoutUsers,
                SuspensionsPerDay = GenerateStatsPerDay(channelReport),
                TagAppearances = channelReport.TagAppearances.Select(x => new TagCountModel { Tag = x.Key.Map(), Count = x.Value }).ToList(),
                TotalBans = channelReport.TotalBans,
                TotalSuspensions = channelReport.TotalSuspensions,
                TotalTimeouts = channelReport.TotalTimeouts,
                UniqueUsers = channelReport.UniqueUsers,
                UniqueUsersBan = channelReport.UniqueUsersBan,
                UniqueUsersSuspensions = channelReport.UniqueUsersSuspensions,
                UniqueUsersTimeout = channelReport.UniqueUsersTimeout,
            };
        }

        private static List<StatsPerDay> GenerateStatsPerDay(ChannelReport channelReport)
        {
            var result = new List<StatsPerDay>();

            var suspensionCount = channelReport.SuspensionsPerDay.OrderBy(x => x.Key).ToList();
            var banCount = channelReport.BansPerDay.OrderBy(x => x.Key).ToList();
            var timeoutCount = channelReport.TimeoutsPerDay.OrderBy(x => x.Key).ToList();

            for (int i = 0; i < suspensionCount.Count; i++)
            {
                var suspensionForDate = suspensionCount[i];
                var banForDate = banCount[i];
                var timeoutForDate = timeoutCount[i];

                result.Add(new StatsPerDay
                {
                    Date = suspensionForDate.Key,
                    SuspensionsCount = suspensionForDate.Value,
                    BansCount = banForDate.Value,
                    TimeoutCount = timeoutForDate.Value
                });
            }

            return result;
        }
    }
}
