using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Messaging.Commands
{
    public class SendDiscordNotificaitonCommand
    {
        public string Username { get; set; }
        public string ChannelOfOrigin { get; set; }
        public Guid RuleId { get; set; }
    }
}
