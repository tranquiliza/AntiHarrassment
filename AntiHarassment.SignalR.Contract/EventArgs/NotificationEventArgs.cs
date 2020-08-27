using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.SignalR.Contract.EventArgs
{
    public class NotificationEventArgs
    {
        public string Username { get; set; }
        public string RuleName { get; set; }
        public string ChannelOfOrigin { get; set; }
    }
}
