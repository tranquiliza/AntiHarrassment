using AntiHarassment.Contract;
using AntiHarassment.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace AntiHarassment.WebApi.Mappers
{
    public static class SystemReportMapper
    {
        public static SystemReportModel Map(this SystemReport systemReport)
        {
            return new SystemReportModel
            {
                UnauditedSuspensionsCount = systemReport.UnauditedSuspensionsCount,
                SuspendedUsers = systemReport.SuspendedUsers,
                BannedUsers = systemReport.BannedUsers,
                TimedoutUsers = systemReport.TimedoutUsers,
                ValidSuspensionsPerDay = MapValidSuspensionsStatsPerDay(systemReport),
                InvalidSuspensionsPerDay = MapInvalidSuspensionsStatsPerDay(systemReport),
                UniqueUsers = systemReport.UniqueUsers,
                ValidTotalSuspensions = systemReport.ValidTotalSuspensions,
                ValidTotalBans = systemReport.ValidTotalBans,
                ValidTotalTimeouts = systemReport.ValidTotalTimeouts,
                ValidUniqueUsersSuspensions = systemReport.ValidUniqueUsersSuspensions,
                ValidUniqueUsersBan = systemReport.ValidUniqueUsersBan,
                ValidUniqueUsersTimeout = systemReport.ValidUniqueUsersTimeout,
                InvalidTotalSuspensions = systemReport.InvalidTotalSuspensions,
                InvalidTotalBans = systemReport.InvalidTotalBans,
                InvalidTotalTimeouts = systemReport.InvalidTotalTimeouts,
                InvalidUniqueUsersSuspensions = systemReport.InvalidUniqueUsersSuspensions,
                InvalidUniqueUsersBan = systemReport.InvalidUniqueUsersBan,
                InvalidUniqueUsersTimeout = systemReport.InvalidUniqueUsersTimeout,
                TagAppearances = systemReport.TagAppearances.Select(x => new TagCountModel { Tag = x.Key.Map(), Count = x.Value }).ToList(),
                UniqueUsersSuspendedBySystem = systemReport.UniqueUsersSuspendedBySystem
            };
        }

        private static List<StatsPerDay> MapValidSuspensionsStatsPerDay(SystemReport systemReport)
        {
            var result = new List<StatsPerDay>();

            var suspensionCount = systemReport.ValidSuspensionsPerDay.OrderBy(x => x.Key).ToList();
            var banCount = systemReport.ValidBansPerDay.OrderBy(x => x.Key).ToList();
            var timeoutCount = systemReport.ValidTimeoutsPerDay.OrderBy(x => x.Key).ToList();

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

        private static List<StatsPerDay> MapInvalidSuspensionsStatsPerDay(SystemReport systemReport)
        {
            var result = new List<StatsPerDay>();

            var suspensionCount = systemReport.InvalidSuspensionsPerDay.OrderBy(x => x.Key).ToList();
            var banCount = systemReport.InvalidBansPerDay.OrderBy(x => x.Key).ToList();
            var timeoutCount = systemReport.InvalidTimeoutsPerDay.OrderBy(x => x.Key).ToList();

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
