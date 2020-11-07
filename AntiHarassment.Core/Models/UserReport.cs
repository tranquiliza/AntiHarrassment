using System.Collections.Generic;
using System.Linq;

namespace AntiHarassment.Core.Models
{
    public class UserReport
    {
        private class TagCount
        {
            public TagCount(Tag tag, int count)
            {
                Tag = tag;
                Count = count;
            }

            public Tag Tag { get; }
            public int Count { get; }
        }

        public string Username { get; private set; }

        public List<Suspension> Suspensions { get; private set; }

        public List<Tag> Tags => Suspensions.SelectMany(x => x.Tags).ToList().GroupBy(x => x.TagName).Select(group => group.First()).ToList();

        private readonly List<TagCount> BanTagCounts = new List<TagCount>();

        private readonly List<TagCount> TimeoutTagCounts = new List<TagCount>();

        public List<string> BannedFromChannels => Suspensions.GroupBy(x => x.ChannelOfOrigin)
            .Select(groupedSuspensions => groupedSuspensions.FirstOrDefault(y => y.SuspensionType == SuspensionType.Ban)?.ChannelOfOrigin).Where(x => x != null).ToList();

        public List<string> TimedOutFromChannels => Suspensions.GroupBy(x => x.ChannelOfOrigin)
            .Select(groupedSuspensions => groupedSuspensions.FirstOrDefault(y => y.SuspensionType == SuspensionType.Timeout)?.ChannelOfOrigin).Where(x => x != null).ToList();

        public bool Exceeds(ChannelRule rule)
        {
            var bannedTagCount = BanTagCounts.Find(x => x.Tag.TagId == rule.Tag.TagId);
            if (bannedTagCount != null && bannedTagCount.Count >= rule.BansForTrigger && rule.BansForTrigger != 0)
                return true;

            var timedoutTagCount = TimeoutTagCounts.Find(x => x.Tag.TagId == rule.Tag.TagId);
            return timedoutTagCount != null && timedoutTagCount.Count >= rule.TimeoutsForTrigger && rule.TimeoutsForTrigger != 0;
        }

        public List<string> AssociatatedAccounts => Suspensions.SelectMany(x => x.LinkedUsers.Select(y => y.Username)).Distinct().ToList();

        public UserReport(string username, List<Suspension> suspensions)
        {
            Username = username;
            Suspensions = suspensions.Where(x => !x.InvalidSuspension && x.Audited).ToList();

            // As these are only used internally, and for calculation of automated ban triggers, we cannot include system as a source!
            var allBannedTags = Suspensions.Where(x => x.SuspensionType == SuspensionType.Ban && x.SuspensionSource != SuspensionSource.System).SelectMany(x => x.Tags);
            foreach (var group in allBannedTags.GroupBy(x => x.TagId))
            {
                var tag = group.FirstOrDefault();
                if (tag != null)
                    BanTagCounts.Add(new TagCount(tag, group.Count()));
            }

            // As these are only used internally, and for calculation of automated ban triggers, we cannot include system as a source!
            var allTimedoutTags = Suspensions.Where(x => x.SuspensionType == SuspensionType.Timeout && x.SuspensionSource != SuspensionSource.System).SelectMany(x => x.Tags);
            foreach (var group in allTimedoutTags.GroupBy(x => x.TagId))
            {
                var tag = group.FirstOrDefault();
                if (tag != null)
                    TimeoutTagCounts.Add(new TagCount(tag, group.Count()));
            }
        }
    }
}
