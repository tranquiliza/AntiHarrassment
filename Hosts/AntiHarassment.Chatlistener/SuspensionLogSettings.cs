using AntiHarassment.Chatlistener.Core;
using System;

namespace AntiHarassment.Chatlistener
{
    public class SuspensionLogSettings : ISuspensionLogSettings
    {
        public TimeSpan TimeoutChatLogRecordTime { get; }

        public TimeSpan BanChatLogRecordTime { get; }

        public int MinimumDurationForTimeouts { get; }

        public SuspensionLogSettings(TimeSpan timeoutChatLogRecordTime, TimeSpan banChatLogRecordTime, int minimumDurationForTimeouts)
        {
            TimeoutChatLogRecordTime = timeoutChatLogRecordTime;
            BanChatLogRecordTime = banChatLogRecordTime;
            MinimumDurationForTimeouts = minimumDurationForTimeouts;
        }
    }
}
