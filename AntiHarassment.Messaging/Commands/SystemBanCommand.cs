using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Messaging.Commands
{
    public class SystemBanCommand
    {
        public string Username { get; set; }
        public string ChannelToBanFrom { get; set; }
        public string SystemReason { get; set; }

        public SystemBanCommand(string username, string channelToBanFrom, string systemReason)
        {
            Username = username;
            ChannelToBanFrom = channelToBanFrom;
            SystemReason = systemReason;
        }
    }
}
