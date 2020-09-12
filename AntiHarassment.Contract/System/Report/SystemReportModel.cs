using System.Collections.Generic;

namespace AntiHarassment.Contract
{
    public class SystemReportModel
    {
        public int UnauditedSuspensionsCount { get; set; }

        public List<string> SuspendedUsers { get; set; }
        public List<string> BannedUsers { get; set; }
        public List<string> TimedoutUsers { get; set; }

        public List<StatsPerDay> ValidSuspensionsPerDay { get; set; }
        public List<StatsPerDay> InvalidSuspensionsPerDay { get; set; }

        public List<TagCountModel> TagAppearances { get; set; }

        public int UniqueUsers { get; set; }

        public int ValidTotalSuspensions { get; set; }
        public int ValidTotalBans { get; set; }
        public int ValidTotalTimeouts { get; set; }

        public int ValidUniqueUsersSuspensions { get; set; }
        public int ValidUniqueUsersTimeout { get; set; }
        public int ValidUniqueUsersBan { get; set; }

        public int InvalidTotalSuspensions { get; set; }
        public int InvalidTotalBans { get; set; }
        public int InvalidTotalTimeouts { get; set; }

        public int InvalidUniqueUsersSuspensions { get; set; }
        public int InvalidUniqueUsersTimeout { get; set; }
        public int InvalidUniqueUsersBan { get; set; }

        public int UniqueUsersSuspendedBySystem { get; set; }
    }
}
