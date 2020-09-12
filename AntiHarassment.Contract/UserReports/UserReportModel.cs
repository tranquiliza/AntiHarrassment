using AntiHarassment.Contract.Tags;
using System.Collections.Generic;

namespace AntiHarassment.Contract
{
    public class UserReportModel
    {
        public string Username { get; set; }
        public List<SuspensionModel> Suspensions { get; set; }
        public List<TagModel> Tags { get; set; }
        public List<string> BannedFromChannels { get; set; }
        public List<string> TimedoutFromChannels { get; set; }
        public List<string> AssociatedAccounts { get; set; }
    }
}
