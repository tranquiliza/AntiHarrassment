using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Contract
{
    public class SuspensionModel
    {
        public Guid SuspensionId { get; set; }
        public string Username { get; set; }
        public string ChannelOfOrigin { get; set; }
        public DateTime Timestamp { get; set; }
        public int Duration { get; set; }
        public SuspensionTypeModel SuspensionType { get; set; }
        public List<ChatMessageModel> Messages { get; set; }
    }
}
