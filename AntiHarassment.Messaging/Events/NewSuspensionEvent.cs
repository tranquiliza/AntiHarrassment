using System;

namespace AntiHarassment.Messaging.Events
{
    public class NewSuspensionEvent
    {
        public Guid SuspensionId { get; set; }
        public string ChannelOfOrigin { get; set; }
    }
}
