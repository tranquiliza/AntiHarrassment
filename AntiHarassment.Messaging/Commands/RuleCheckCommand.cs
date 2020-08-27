using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Messaging.Commands
{
    public class RuleCheckCommand
    {
        public string TwitchUsername { get; set; }
        public string ChannelOfOrigin { get; set; }
    }
}
