using System;

namespace AntiHarassment.Messaging.Events
{
    public class SuspensionUpdatedEvent
    {
        public Guid SuspensionId { get; set; }
        public string ChannelOfOrigin { get; set; }
    }
}
