using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Contract
{
    public class UserRulesExceededModel
    {
        public string Username { get; set; }
        public List<ChannelRuleModel> RulesBroken { get; set; }
    }
}
