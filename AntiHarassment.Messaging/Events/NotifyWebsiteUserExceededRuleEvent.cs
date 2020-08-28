using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Messaging.Events
{
    public class NotifyWebsiteUserExceededRuleEvent
    {
        public string Username { get; set; }
        public string ChannelOfOrigin { get; set; }
        public string RuleName { get; set; }

        public NotifyWebsiteUserExceededRuleEvent(string username, string channelOfOrigin, string ruleName)
        {
            Username = username;
            ChannelOfOrigin = channelOfOrigin;
            RuleName = ruleName;
        }
    }
}
