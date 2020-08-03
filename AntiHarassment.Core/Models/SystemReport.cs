using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public class SystemReport
    {
        public List<Suspension> AuditedSuspensions { get; private set; }
        public List<Suspension> UnauditedSuspensions { get; private set; }

        public List<string> SuspendedUsers => AuditedSuspensions.DistinctBy(x => x.Username).Select(x => x.Username).ToList();

        public List<string> BannedUsers => AuditedSuspensions.DistinctBy(x => x.Username)
            .Where(y => y.SuspensionType == SuspensionType.Ban)
            .Select(x => x.Username).ToList();

        public List<string> TimedoutUsers => AuditedSuspensions.DistinctBy(x => x.Username)
            .Where(y => y.SuspensionType == SuspensionType.Timeout)
            .Select(x => x.Username).ToList();

        public Dictionary<DateTime, int> ValidSuspensionsPerDay { get; private set; } = new Dictionary<DateTime, int>();
        public Dictionary<DateTime, int> ValidBansPerDay { get; private set; } = new Dictionary<DateTime, int>();
        public Dictionary<DateTime, int> ValidTimeoutsPerDay { get; private set; } = new Dictionary<DateTime, int>();

        public Dictionary<DateTime, int> InvalidSuspensionsPerDay { get; private set; } = new Dictionary<DateTime, int>();
        public Dictionary<DateTime, int> InvalidBansPerDay { get; private set; } = new Dictionary<DateTime, int>();
        public Dictionary<DateTime, int> InvalidTimeoutsPerDay { get; private set; } = new Dictionary<DateTime, int>();

        public Dictionary<Tag, int> TagAppearances { get; private set; } = new Dictionary<Tag, int>();

        public int UniqueUsers { get; private set; }

        public int ValidTotalSuspensions { get; private set; }
        public int ValidTotalBans { get; private set; }
        public int ValidTotalTimeouts { get; private set; }

        public int ValidUniqueUsersSuspensions { get; private set; }
        public int ValidUniqueUsersTimeout { get; private set; }
        public int ValidUniqueUsersBan { get; private set; }

        public int InvalidTotalSuspensions { get; private set; }
        public int InvalidTotalBans { get; private set; }
        public int InvalidTotalTimeouts { get; private set; }

        public int InvalidUniqueUsersSuspensions { get; private set; }
        public int InvalidUniqueUsersTimeout { get; private set; }
        public int InvalidUniqueUsersBan { get; private set; }

        public SystemReport(List<Suspension> unauditedSuspensions, List<Suspension> auditedSuspensions, int uniqueUsers)
        {
            AuditedSuspensions = auditedSuspensions;
            UniqueUsers = uniqueUsers;

            ValidTotalSuspensions = auditedSuspensions.Count(x => !x.InvalidSuspension);
            ValidTotalBans = auditedSuspensions.Count(x => x.SuspensionType == SuspensionType.Ban && !x.InvalidSuspension);
            ValidTotalTimeouts = auditedSuspensions.Count(x => x.SuspensionType == SuspensionType.Timeout && !x.InvalidSuspension);

            ValidUniqueUsersSuspensions = auditedSuspensions.DistinctBy(x => x.Username).Count(x => !x.InvalidSuspension);
            ValidUniqueUsersTimeout = auditedSuspensions.DistinctBy(x => x.Username).Count(x => x.SuspensionType == SuspensionType.Timeout && !x.InvalidSuspension);
            ValidUniqueUsersBan = auditedSuspensions.DistinctBy(x => x.Username).Count(x => x.SuspensionType == SuspensionType.Ban && !x.InvalidSuspension);

            InvalidTotalSuspensions = auditedSuspensions.Count(x => x.InvalidSuspension);
            InvalidTotalBans = auditedSuspensions.Count(x => x.SuspensionType == SuspensionType.Ban && x.InvalidSuspension);
            InvalidTotalTimeouts = auditedSuspensions.Count(x => x.SuspensionType == SuspensionType.Timeout && x.InvalidSuspension);

            InvalidUniqueUsersSuspensions = auditedSuspensions.DistinctBy(x => x.Username).Count(x => x.InvalidSuspension);
            InvalidUniqueUsersTimeout = auditedSuspensions.DistinctBy(x => x.Username).Count(x => x.SuspensionType == SuspensionType.Timeout && x.InvalidSuspension);
            InvalidUniqueUsersBan = auditedSuspensions.DistinctBy(x => x.Username).Count(x => x.SuspensionType == SuspensionType.Ban && x.InvalidSuspension);

            var suspensionsOnDate = new Dictionary<DateTime, List<Suspension>>();
            foreach (var group in auditedSuspensions.GroupBy(x => x.Timestamp.Date))
                suspensionsOnDate.Add(group.Key, group.ToList());

            foreach (var suspensionDate in suspensionsOnDate)
            {
                ValidSuspensionsPerDay.Add(suspensionDate.Key, suspensionDate.Value.Count(x => !x.InvalidSuspension));
                ValidBansPerDay.Add(suspensionDate.Key, suspensionDate.Value.Count(x => x.SuspensionType == SuspensionType.Ban && !x.InvalidSuspension));
                ValidTimeoutsPerDay.Add(suspensionDate.Key, suspensionDate.Value.Count(x => x.SuspensionType == SuspensionType.Timeout && !x.InvalidSuspension));

                InvalidSuspensionsPerDay.Add(suspensionDate.Key, suspensionDate.Value.Count(x => x.InvalidSuspension));
                InvalidBansPerDay.Add(suspensionDate.Key, suspensionDate.Value.Count(x => x.SuspensionType == SuspensionType.Ban && x.InvalidSuspension));
                InvalidTimeoutsPerDay.Add(suspensionDate.Key, suspensionDate.Value.Count(x => x.SuspensionType == SuspensionType.Timeout && x.InvalidSuspension));
            }

            foreach (var group in auditedSuspensions.Where(x => !x.InvalidSuspension).SelectMany(x => x.Tags).GroupBy(x => x.TagId))
                TagAppearances.Add(group.First(), group.Count());
            UnauditedSuspensions = unauditedSuspensions;
        }
    }
}
