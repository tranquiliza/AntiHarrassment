using System;

namespace AntiHarassment.Chatlistener.Core
{
    public interface ISuspensionLogSettings
    {
        public TimeSpan TimeoutChatLogRecordTime { get; }
        public TimeSpan BanChatLogRecordTime { get; }
        public int MinimumDurationForTimeouts { get; }
    }
}
