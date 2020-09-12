using System;

namespace AntiHarassment.Contract
{
    public class ChatMessageModel
    {
        public string Username { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public bool AutoModded { get; set; }
    }
}
