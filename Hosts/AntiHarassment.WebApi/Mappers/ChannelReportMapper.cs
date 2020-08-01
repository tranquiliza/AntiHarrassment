using AntiHarassment.Contract;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Mappers
{
    public static class ChannelReportMapper
    {
        public static ChannelReportModel Map(this ChannelReport channelReport)
        {
            return new ChannelReportModel
            {
                ChannelName = channelReport.ChannelName,
                UserReports = channelReport.UserReports.Map(),
                SuspendedUsers = channelReport.SuspendedUsers,
                BannedUsers = channelReport.BannedUsers,
                TimedoutUsers = channelReport.TimedoutUsers,
                SuspensionsPerDay = channelReport.SuspensionsPerDay.Select(x => new DayCounterModel {Date = x.Key, Count = x.Value }).ToList(),
                BansPerDay = channelReport.BansPerDay.Select(x => new DayCounterModel { Date = x.Key, Count = x.Value }).ToList(),
                TimeoutsPerDay = channelReport.TimeoutsPerDay.Select(x => new DayCounterModel { Date = x.Key, Count = x.Value }).ToList(),
                TagAppearances = channelReport.TagAppearances.Select(x => new TagCountModel { Tag = x.Key.Map(), Count = x.Value}).ToList()
            };
        }
    }
}
