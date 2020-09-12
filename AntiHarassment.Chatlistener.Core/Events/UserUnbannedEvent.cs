using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Chatlistener.Core.Events
{
    public class UserUnbannedEvent
    {
        public string Username { get; set; }
        public string UnbannedBy { get; set; }
        public string Channel { get; set; }
    }
}
