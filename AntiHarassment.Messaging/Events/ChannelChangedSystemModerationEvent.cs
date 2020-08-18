using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Messaging.Events
{
    public class ChannelChangedSystemModerationEvent
    {
        public string ChannelName { get; set; }
        public bool SystemIsModerator { get; set; }
        public Guid RequestedByUserId { get; set; }
    }
}
