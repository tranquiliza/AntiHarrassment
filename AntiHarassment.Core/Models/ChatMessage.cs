using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public class ChatMessage
    {
        public DateTime Timestamp { get; private set; }
        public string Message { get; private set; }

        public ChatMessage(DateTime timestamp, string message)
        {
            Timestamp = timestamp;
            Message = message;
        }
    }
}
