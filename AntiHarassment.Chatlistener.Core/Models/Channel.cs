using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Chatlistener.Core.Models
{
    public class Channel
    {
        public string ChannelName { get; private set; }
        public bool ShouldListen { get; private set; }

        public Channel(string channelName, bool shouldListen)
        {
            ChannelName = channelName;
            ShouldListen = shouldListen;
        }
    }
}
