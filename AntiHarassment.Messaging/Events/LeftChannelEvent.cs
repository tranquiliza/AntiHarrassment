using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Messaging.Events
{
    public class LeftChannelEvent
    {
        public string ChannelName { get; set; }

        public LeftChannelEvent(string channelName)
        {
            ChannelName = channelName;
        }
    }
}
