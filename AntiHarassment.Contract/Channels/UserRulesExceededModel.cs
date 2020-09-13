using System.Collections.Generic;

namespace AntiHarassment.Contract
{
    public class UserRulesExceededModel
    {
        public string Username { get; set; }
        public List<ChannelRuleModel> RulesBroken { get; set; }
    }
}
