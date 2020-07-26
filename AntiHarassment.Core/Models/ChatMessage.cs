using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public class ChatMessage
    {
        public string Username { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Message { get; private set; }
        public bool AutoModded { get; private set; }

        private ChatMessage() { }

        public ChatMessage(DateTime timestamp, string message, bool autoModded)
        {
            Timestamp = timestamp;
            Message = message;
            AutoModded = autoModded;
        }

        public void AttachUsername(string username)
        {
            Username = username;
        }
    }
}
