using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.SignalR.Contract.EventArgs
{
    public class SuspensionUpdatedEventArgs
    {
        public Guid SuspensionId { get; set; }
        public string ChannelOfOrigin { get; set; }
    }
}
