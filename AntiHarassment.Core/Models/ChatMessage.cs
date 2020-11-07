using Newtonsoft.Json;
using System;

namespace AntiHarassment.Core.Models
{
    public class ChatMessage
    {
        [JsonProperty]
        public Guid ChatMessageId { get; set; }

        [JsonProperty]
        public string TwitchMessageId { get; set; }

        [JsonProperty]
        public string Username { get; private set; }

        [JsonProperty]
        public string ChannelOfOrigin { get; set; }

        [JsonProperty]
        public DateTime Timestamp { get; private set; }

        [JsonProperty]
        public string Message { get; private set; }

        [JsonProperty]
        public bool AutoModded { get; private set; }

        [JsonProperty]
        public bool Deleted { get; private set; }

        private ChatMessage() { }

        public ChatMessage(
            DateTime timestamp,
            string twitchMessageId,
            string username,
            string channelOfOrigin,
            string message,
            bool autoModded,
            bool deleted)
        {
            ChatMessageId = Guid.NewGuid();
            Timestamp = timestamp;
            TwitchMessageId = twitchMessageId;
            Username = username;
            ChannelOfOrigin = channelOfOrigin;
            Message = message;
            AutoModded = autoModded;
            Deleted = deleted;
        }

        public void MarkDeleted()
        {
            Deleted = true;
        }
    }
}
