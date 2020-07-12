using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Contract
{
    public class ChatMessageModel
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }
}
