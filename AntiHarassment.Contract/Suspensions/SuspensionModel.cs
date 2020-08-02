using AntiHarassment.Contract.Tags;
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
        public bool InvalidSuspension { get; set; }
        public string InvalidationReason { get; set; }
        public bool Audited { get; set; }
        public SuspensionTypeModel SuspensionType { get; set; }
        public SuspensionSourceModel SuspensionSource { get; set; }
        public List<TagModel> Tags { get; set; }
        public List<ChatMessageModel> Messages { get; set; }
        public List<string> LinkedUsernames { get; set; }
    }
}
