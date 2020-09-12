using System;

namespace AntiHarassment.Messaging.Commands
{
    public class LeaveChannelCommand
    {
        public Guid RequestedByUserId { get; set; }
        public string ChannelName { get; set; }
    }
}
