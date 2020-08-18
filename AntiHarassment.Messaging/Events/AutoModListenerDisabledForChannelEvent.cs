using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Messaging.Events
{
    public class AutoModListenerDisabledForChannelEvent
    {
        public string ChannelName { get; set; }

        public AutoModListenerDisabledForChannelEvent(string channelName)
        {
            ChannelName = channelName;
        }
    }
}
