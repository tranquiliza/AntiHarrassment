using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public class ChannelReport
    {
        public string ChannelName { get; private set; }
        public List<Suspension> Suspensions { get; private set; }
        public List<UserReport> UserReports { get; private set; }

        public List<string> SuspendedUsers => Suspensions.DistinctBy(x => x.Username).Select(x => x.Username).ToList();

        public List<string> BannedUsers => Suspensions.DistinctBy(x => x.Username)
            .Where(y => y.SuspensionType == SuspensionType.Ban)
            .Select(x => x.Username).ToList();

        public List<string> TimedoutUsers => Suspensions.DistinctBy(x => x.Username)
            .Where(y => y.SuspensionType == SuspensionType.Timeout)
            .Select(x => x.Username).ToList();

        public Dictionary<DateTime, int> SuspensionsPerDay { get; private set; } = new Dictionary<DateTime, int>();
        public Dictionary<DateTime, int> BansPerDay { get; private set; } = new Dictionary<DateTime, int>();
        public Dictionary<DateTime, int> TimeoutsPerDay { get; private set; } = new Dictionary<DateTime, int>();
        public Dictionary<Tag, int> TagAppearances { get; private set; } = new Dictionary<Tag, int>();

        public int TotalSuspensions { get; private set; }
        public int TotalBans { get; private set; }
        public int TotalTimeouts { get; private set; }

        public int UniqueUsers { get; private set; }

        public int UniqueUsersSuspensions { get; private set; }
        public int UniqueUsersTimeout { get; private set; }
        public int UniqueUsersBan { get; private set; }

        public ChannelReport(string channelName, List<Suspension> suspensions, int uniqueUserCountForChannel, List<UserReport> userReports)
        {
            ChannelName = channelName;
            Suspensions = suspensions;

            TotalSuspensions = suspensions.Count;
            TotalBans = suspensions.Count(x => x.SuspensionType == SuspensionType.Ban);
            TotalTimeouts = suspensions.Count(x => x.SuspensionType == SuspensionType.Timeout);
            UniqueUsers = uniqueUserCountForChannel;

            UniqueUsersSuspensions = suspensions.DistinctBy(x => x.Username).Count();
            UniqueUsersTimeout = suspensions.DistinctBy(x => x.Username).Count(x => x.SuspensionType == SuspensionType.Timeout);
            UniqueUsersBan = suspensions.DistinctBy(x => x.Username).Count(x => x.SuspensionType == SuspensionType.Ban);

            var suspensionsOnDate = new Dictionary<DateTime, List<Suspension>>();
            foreach (var group in suspensions.GroupBy(x => x.Timestamp.Date))
                suspensionsOnDate.Add(group.Key, group.ToList());

            foreach (var suspensionDate in suspensionsOnDate)
            {
                SuspensionsPerDay.Add(suspensionDate.Key, suspensionDate.Value.Count);
                BansPerDay.Add(suspensionDate.Key, suspensionDate.Value.Count(x => x.SuspensionType == SuspensionType.Ban));
                TimeoutsPerDay.Add(suspensionDate.Key, suspensionDate.Value.Count(x => x.SuspensionType == SuspensionType.Timeout));
            }

            foreach (var group in suspensions.SelectMany(x => x.Tags).GroupBy(x => x.TagId))
                TagAppearances.Add(group.First(), group.Count());
            UserReports = userReports;
        }
    }
}
