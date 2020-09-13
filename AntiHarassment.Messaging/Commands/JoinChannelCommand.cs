using System;

namespace AntiHarassment.Messaging.Commands
{
    public class JoinChannelCommand
    {
        public Guid RequestedByUserId { get; set; }
        public string ChannelName { get; set; }
    }
}
