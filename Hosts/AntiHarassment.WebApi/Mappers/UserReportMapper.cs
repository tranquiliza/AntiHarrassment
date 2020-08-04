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
        public static List<UserReportModel> Map(this List<UserReport> userReports, string baseUrl)
            => userReports.Select(x => x.Map(baseUrl)).ToList();

        public static UserReportModel Map(this UserReport userReport, string baseUrl)
        {
            return new UserReportModel
            {
                Username = userReport.Username,
                BannedFromChannels = userReport.BannedFromChannels,
                TimedoutFromChannels = userReport.TimedOutFromChannels,
                AssociatedAccounts = userReport.AssociatatedAccounts,
                Suspensions = userReport.Suspensions.Map(baseUrl),
                Tags = userReport.Tags.Map()
            };
        }
    }
}
