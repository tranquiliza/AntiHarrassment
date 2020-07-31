using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Contract
{
    public class CreateSuspensionModel
    {
        public string TwitchUsername { get; set; }
        public string ChannelOfOrigin { get; set; }
    }
}
