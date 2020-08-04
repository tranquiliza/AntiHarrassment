using AntiHarassment.Core.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public sealed class Suspension : DomainBase
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
        public SuspensionSource SuspensionSource { get; private set; }

        [JsonProperty]
        public string CreatedByUser { get; private set; }

        [JsonProperty]
        public DateTime Timestamp { get; private set; }

        [JsonProperty]
        public bool InvalidSuspension { get; private set; }

        [JsonProperty]
        public string InvalidationReason { get; private set; }

        [JsonProperty]
        public bool Audited { get; private set; }

        [JsonProperty]
        private List<Tag> tags { get; set; } = new List<Tag>();

        [JsonIgnore]
        public IReadOnlyList<Tag> Tags => tags.AsReadOnly();

        [JsonProperty]
        private List<string> linkedUsernames { get; set; } = new List<string>();

        [JsonIgnore]
        public IReadOnlyList<string> LinkedUsernames => linkedUsernames.AsReadOnly();

        [JsonProperty]
        private List<string> images { get; set; } = new List<string>();

        [JsonIgnore]
        public IReadOnlyList<string> Images => images.AsReadOnly();

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

        private Suspension(string username, string channelOfOrigin, DateTime timestamp)
        {
            SuspensionId = Guid.NewGuid();
            Username = username;
            ChannelOfOrigin = channelOfOrigin;
            Timestamp = timestamp;
        }

        public bool UpdateValidity(bool invalidate, string invalidationReason, IApplicationContext context, DateTime timestamp)
        {
            if (invalidate && string.IsNullOrEmpty(invalidationReason))
                return false;

            InvalidSuspension = invalidate;
            InvalidationReason = invalidationReason;

            AddAuditTrail(context, nameof(InvalidSuspension), invalidate, timestamp);

            return true;
        }

        public void UpdateAuditedState(bool audited, IApplicationContext context, DateTime timestamp)
        {
            Audited = audited;

            AddAuditTrail(context, nameof(Audited), audited, timestamp);
        }

        public bool TryAddTag(Tag tag, IApplicationContext context, DateTime timestamp)
        {
            if (tags.Any(x => x.TagId == tag.TagId))
                return false;

            tags.Add(tag);

            AddAuditTrail(context, nameof(tags), tags, timestamp);
            return true;
        }

        public void RemoveTag(Tag tag, IApplicationContext context, DateTime timestamp)
        {
            var existingTag = tags.Find(x => x.TagId == tag.TagId);
            if (existingTag != null)
                tags.Remove(existingTag);

            AddAuditTrail(context, nameof(tags), tags, timestamp);
        }

        public void AddUserLink(string twitchUsername, IApplicationContext context, DateTime timestamp)
        {
            if (linkedUsernames.Contains(twitchUsername))
                return;

            linkedUsernames.Add(twitchUsername);
            AddAuditTrail(context, nameof(linkedUsernames), linkedUsernames, timestamp);
        }

        public void RemoveUserLink(string twitchUsername, IApplicationContext context, DateTime timestamp)
        {
            linkedUsernames.Remove(twitchUsername);

            AddAuditTrail(context, nameof(linkedUsernames), linkedUsernames, timestamp);
        }

        public string AddImage(string fileExtension, IApplicationContext context, DateTime timestamp)
        {
            var uniqueName = Guid.NewGuid().ToString("N") + fileExtension;
            images.Add(uniqueName);

            AddAuditTrail(context, nameof(images), images, timestamp);
            return uniqueName;
        }

        public static Suspension CreateTimeout(string username, string channelOfOrigin, int duration, DateTime timestamp, List<ChatMessage> chatMessages)
        {
            return new Suspension(username, channelOfOrigin, timestamp)
            {
                SuspensionType = SuspensionType.Timeout,
                SuspensionSource = SuspensionSource.System,
                Timestamp = timestamp,
                Duration = duration,
                ChatMessages = chatMessages
            };
        }

        public static Suspension CreateBan(string username, string channelOfOrigin, DateTime timestamp, List<ChatMessage> chatMessages)
        {
            return new Suspension(username, channelOfOrigin, timestamp)
            {
                SuspensionType = SuspensionType.Ban,
                SuspensionSource = SuspensionSource.System,
                Timestamp = timestamp,
                Duration = 0,
                ChatMessages = chatMessages
            };
        }

        public static Suspension CreateManualBan(string username, string channelOfOrigin, DateTime timestamp, string createdBy)
        {
            return new Suspension(username, channelOfOrigin, timestamp)
            {
                Duration = 0,
                SuspensionType = SuspensionType.Ban,
                SuspensionSource = SuspensionSource.User,
                CreatedByUser = createdBy,
                ChatMessages = new List<ChatMessage>()
            };
        }
    }
}
