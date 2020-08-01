using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Contract
{
    public class ChannelReportModel
    {
        public string ChannelName { get; set; }
        public List<UserReportModel> UserReports { get; set; }
        public List<string> SuspendedUsers { get; set; }
        public List<string> BannedUsers { get; set; }
        public List<string> TimedoutUsers { get; set; }
        public List<DayCounterModel> SuspensionsPerDay { get; set; }
        public List<DayCounterModel> BansPerDay { get; set; }
        public List<DayCounterModel> TimeoutsPerDay { get; set; }
        public List<TagCountModel> TagAppearances { get; set; }
    }
}
