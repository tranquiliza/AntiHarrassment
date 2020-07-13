using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Contract
{
    public class ChannelModel
    {
        public Guid ChannelId { get; set; }
        public string ChannelName { get; set; }
        public bool ShouldListen { get; set; }
        public List<string> Moderators { get; set; }
    }
}
