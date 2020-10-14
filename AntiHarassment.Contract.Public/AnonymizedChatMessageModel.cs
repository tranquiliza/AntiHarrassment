using System;

namespace AntiHarassment.Contract.Public
{
    public class AnonymizedChatMessageModel
    {
        public string Message { get; set; }
        public DateTime TimestampUtc { get; set; }
        public bool AutoModded { get; set; }
    }
}
