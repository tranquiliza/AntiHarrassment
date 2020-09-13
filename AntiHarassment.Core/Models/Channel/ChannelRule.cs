using Newtonsoft.Json;
using System;

namespace AntiHarassment.Core.Models
{
    public class ChannelRule : DomainBase
    {
        [JsonProperty]
        public Guid RuleId { get; private set; }

        [JsonProperty]
        public string RuleName { get; private set; }

        [JsonProperty]
        public Tag Tag { get; private set; }

        [JsonProperty]
        public int BansForTrigger { get; private set; }

        [JsonProperty]
        public int TimeoutsForTrigger { get; private set; }

        [JsonProperty]
        public ChannelRuleAction ActionOnTrigger { get; private set; }

        private ChannelRule() { }

        public ChannelRule(string ruleName, Tag tag, int bansForTrigger, int timeoutsForTrigger, ChannelRuleAction actionOnTrigger)
        {
            RuleId = Guid.NewGuid();
            Tag = tag;
            BansForTrigger = bansForTrigger;
            TimeoutsForTrigger = timeoutsForTrigger;
            ActionOnTrigger = actionOnTrigger;
            RuleName = ruleName;
        }

        public void Update(string rulename, int bansForTrigger, int timeoutsForTrigger, ChannelRuleAction channelRuleAction)
        {
            RuleName = rulename;
            BansForTrigger = bansForTrigger;
            TimeoutsForTrigger = timeoutsForTrigger;
            ActionOnTrigger = channelRuleAction;
        }
    }
}
