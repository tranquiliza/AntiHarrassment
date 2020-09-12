using AntiHarassment.Contract.Tags;
using System;

namespace AntiHarassment.Contract
{
    public class ChannelRuleModel
    {
        public Guid RuleId { get; set; }

        public string RuleName { get; set; }

        public TagModel Tag { get; set; }

        public int BansForTrigger { get; set; }

        public int TimeoutsForTrigger { get; set; }

        public ChannelRuleActionModel ActionOnTrigger { get; set; }
    }
}
