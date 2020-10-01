using System;
using System.Collections.Generic;

namespace AntiHarassment.Contract.Public
{
    public class AnonymizedSuspensionModel
    {
        public Guid SuspensionId { get; set; }
        public DateTime TimestampUtc { get; set; }
        public string SuspensionType { get; set; }
        public int DurationInSeconds { get; set; }
        public string SuspensionSource { get; set; }
        public string SystemReason { get; set; }
        public IEnumerable<SimpleTagModel> Tags { get; set; }
        public IEnumerable<AnonymizedChatMessageModel> ChatMessages { get; set; }
    }
}
