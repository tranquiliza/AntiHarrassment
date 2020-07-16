using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public sealed class Suspension
    {
        [JsonProperty]
        public Guid SuspensionId { get; private set; }

        [JsonProperty]
        public string Username { get; private set; }

        [JsonProperty]
        public string ChannelOfOrigin { get; private set; }

        [JsonProperty]
        public SuspensionType SuspensionType { get; private set; }

        [JsonProperty]
        public DateTime Timestamp { get; private set; }

        [JsonProperty]
        public bool InvalidSuspension { get; private set; }

        /// <summary>
        /// Length of the suspension in Seconds: 0 if permanent
        /// </summary>
        [JsonProperty]
        public int Duration { get; private set; }

        /// <summary>
        /// Chat messages leading up to the suspension
        /// </summary>
        [JsonProperty]
        public List<ChatMessage> ChatMessages { get; private set; }

        private Suspension() { }

        public void UpdateValidity(bool invalidate)
        {
            InvalidSuspension = invalidate;
        }

        public static Suspension CreateTimeout(string username, string channelOfOrigin, int duration, DateTime timestamp, List<ChatMessage> chatMessages)
        {
            return new Suspension
            {
                SuspensionId = Guid.NewGuid(),
                Username = username,
                ChannelOfOrigin = channelOfOrigin,
                SuspensionType = SuspensionType.Timeout,
                Timestamp = timestamp,
                Duration = duration,
                ChatMessages = chatMessages
            };
        }

        public static Suspension CreateBan(string username, string channelOfOrigin, DateTime timestamp, List<ChatMessage> chatMessages)
        {
            return new Suspension
            {
                SuspensionId = Guid.NewGuid(),
                Username = username,
                ChannelOfOrigin = channelOfOrigin,
                SuspensionType = SuspensionType.Ban,
                Timestamp = timestamp,
                Duration = 0,
                ChatMessages = chatMessages
            };
        }
    }
}
