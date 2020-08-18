using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Messaging.Events
{
    public class AutoModListenerEnabledForChannelEvent
    {
        public string ChannelName { get; set; }

        public AutoModListenerEnabledForChannelEvent(string channelName)
        {
            ChannelName = channelName;
        }
    }
}
