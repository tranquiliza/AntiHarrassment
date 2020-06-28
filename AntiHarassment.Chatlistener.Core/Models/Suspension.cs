using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AntiHarassment.Chatlistener.Core.Models
{
    public class Suspension
    {
        public string Username { get; private set; }
        public string ChannelOfOrigin { get; private set; }
        public SuspensionType SuspensionType { get; private set; }
        public DateTime Timestamp { get; private set; }
        public int Duration { get; private set; }

        private Suspension() { }

        public static Suspension CreateTimeout(string username, string channelOfOrigin, int duration, DateTime timestamp)
        {
            return new Suspension
            {
                Username = username,
                ChannelOfOrigin = channelOfOrigin,
                SuspensionType = SuspensionType.Timeout,
                Timestamp = timestamp,
                Duration = duration
            };
        }

        public static Suspension CreateBan(string username, string channelOfOrigin, DateTime timestamp)
        {
            return new Suspension
            {
                Username = username,
                ChannelOfOrigin = channelOfOrigin,
                SuspensionType = SuspensionType.Ban,
                Timestamp = timestamp,
                Duration = 0
            };
        }
    }
}
