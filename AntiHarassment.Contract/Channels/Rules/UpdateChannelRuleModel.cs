namespace AntiHarassment.Contract
{
    public class UpdateChannelRuleModel
    {
        public ChannelRuleActionModel ChannelRuleAction { get; set; }
        public string RuleName { get; set; }
        public int BansForTrigger { get; set; }
        public int TimeOutsForTrigger { get; set; }
    }
}
