using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public class ChannelReport
    {
        public string ChannelName { get; private set; }
        public List<UserReport> UserReports { get; private set; }

        public List<string> SuspendedUsers => UserReports.Select(x => x.Username).ToList();

        public List<string> BannedUsers => UserReports
            .Where(x => x.Suspensions.Any(y => y.SuspensionType == SuspensionType.Ban))
            .Select(x => x.Username).ToList();

        public List<string> TimedoutUsers => UserReports
            .Where(x => x.Suspensions.Any(y => y.SuspensionType == SuspensionType.Timeout))
            .Select(x => x.Username).ToList();

        public Dictionary<DateTime, int> SuspensionsPerDay { get; private set; } = new Dictionary<DateTime, int>();
        public Dictionary<DateTime, int> BansPerDay { get; private set; } = new Dictionary<DateTime, int>();
        public Dictionary<DateTime, int> TimeoutsPerDay { get; private set; } = new Dictionary<DateTime, int>();
        public Dictionary<Tag, int> TagAppearances { get; private set; } = new Dictionary<Tag, int>();

        public ChannelReport(string channelName, List<UserReport> userReports)
        {
            ChannelName = channelName;
            UserReports = userReports;

            foreach (var group in userReports.SelectMany(x => x.Suspensions).GroupBy(x => x.Timestamp.Date))
            {
                SuspensionsPerDay.Add(group.Key, group.Count());
                BansPerDay.Add(group.Key, group.Count(x => x.SuspensionType == SuspensionType.Ban));
                TimeoutsPerDay.Add(group.Key, group.Count(x => x.SuspensionType == SuspensionType.Timeout));
            }

            foreach (var tagGroup in userReports.SelectMany(x => x.Suspensions).SelectMany(x => x.Tags).GroupBy(x => x.TagId))
            {
                TagAppearances.Add(tagGroup.First(), tagGroup.Count());
            }
        }
    }
}
