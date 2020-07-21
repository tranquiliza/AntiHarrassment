using AntiHarassment.Contract;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Mappers
{
    public static class UserReportMapper
    {
        public static UserReportModel Map(this UserReport userReport)
        {
            return new UserReportModel
            {
                Username = userReport.Username,
                BannedFromChannels = userReport.BannedFromChannels,
                TimedoutFromChannels = userReport.TimedOutFromChannels,
                Suspensions = userReport.Suspensions.Map(),
                Tags = userReport.Tags.Map()
            };
        }
    }
}
