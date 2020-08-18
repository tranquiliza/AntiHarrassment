using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Messaging.Events
{
    public class SuspensionAuditedEvent
    {
        public Guid SuspensionId { get; set; }
        public string TwitchUsername { get; set; }
        public string ChannelOfOrigin { get; set; }
    }
}
