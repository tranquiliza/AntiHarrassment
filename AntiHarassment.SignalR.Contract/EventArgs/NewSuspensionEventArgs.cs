﻿using System;

namespace AntiHarassment.SignalR.Contract.EventArgs
{
    public class NewSuspensionEventArgs
    {
        public Guid SuspensionId { get; set; }
        public string ChannelOfOrigin { get; set; }
    }
}
