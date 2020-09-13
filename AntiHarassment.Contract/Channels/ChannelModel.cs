using System;
using System.Collections.Generic;

namespace AntiHarassment.Contract
{
    public class ChannelModel
    {
        public Guid ChannelId { get; set; }
        public string ChannelName { get; set; }
        public bool ShouldListen { get; set; }
        public bool SystemIsModerator { get; set; }
        public bool ShouldListenForAutoModdedMessages { get; set; }
        public List<string> Moderators { get; set; }
        public List<ChannelRuleModel> ChannelRules { get; set; }
    }
}
