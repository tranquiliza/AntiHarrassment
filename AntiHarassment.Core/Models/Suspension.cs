using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public class Suspension
    {
        [JsonProperty]
        public string Username { get; private set; }

        [JsonProperty]
        public string ChannelOfOrigin { get; private set; }

        [JsonProperty]
        public SuspensionType SuspensionType { get; private set; }

        [JsonProperty]
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Length of the suspension in Milliseconds: 0 if permanent
        /// </summary>
        [JsonProperty]
        public int Duration { get; private set; }

        /// <summary>
        /// Chat messages leading up to the suspension
        /// </summary>
        [JsonProperty]
        public List<ChatMessage> ChatMessages { get; private set; }

        private Suspension() { }

        public static Suspension CreateTimeout(string username, string channelOfOrigin, int duration, DateTime timestamp, List<ChatMessage> chatMessages)
        {
            return new Suspension
            {
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
