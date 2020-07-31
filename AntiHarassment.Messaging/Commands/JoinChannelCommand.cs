using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Messaging.Commands
{
    public class JoinChannelCommand
    {
        public Guid RequestedByUserId { get; set; }
        public string ChannelName { get; set; }
    }
}
