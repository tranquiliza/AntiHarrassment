using System.Collections.Generic;

namespace AntiHarassment.Contract
{
    public class ChannelReportModel
    {
        public string ChannelName { get; set; }
        public List<string> SuspendedUsers { get; set; }
        public List<string> BannedUsers { get; set; }
        public List<string> TimedoutUsers { get; set; }
        public List<StatsPerDay> SuspensionsPerDay { get; set; }
        public List<TagCountModel> TagAppearances { get; set; }

        public int TotalSuspensions { get; set; }
        public int TotalBans { get; set; }
        public int TotalTimeouts { get; set; }

        public int UniqueUsers { get; set; }

        public int UniqueUsersSuspensions { get; set; }
        public int UniqueUsersTimeout { get; set; }
        public int UniqueUsersBan { get; set; }
    }
}
