using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Messaging.Events
{
    public class UserEnteredChannelEvent
    {
        public string TwitchUsername { get; set; }
        public string ChannelOfOrigin { get; set; }
    }
}
