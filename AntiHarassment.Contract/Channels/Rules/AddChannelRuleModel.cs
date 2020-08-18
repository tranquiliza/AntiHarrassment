using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Contract
{
    public class AddChannelRuleModel
    {
        public string RuleName { get; set; }
        public Guid TagId { get; set; }
        public int BansForTrigger { get; set; }
        public int TimeoutsForTrigger { get; set; }
        public ChannelRuleActionModel ChannelRuleAction { get; set; }
    }
}
