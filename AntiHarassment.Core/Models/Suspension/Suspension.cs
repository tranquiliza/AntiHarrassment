using AntiHarassment.Core.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
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
        public string SystemReason { get; private set; }

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
        private List<LinkedUser> linkedUsers { get; set; } = new List<LinkedUser>();

        [JsonIgnore]
        public IReadOnlyList<LinkedUser> LinkedUsers => linkedUsers.AsReadOnly();

        // TODO REMOVE IN V1.7.0
        [JsonProperty]
        private List<string> linkedUsernames { get; set; } = new List<string>();

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

        // TODO REMOVE IN V1.7.0
        public void MigrateData()
        {
            foreach (var username in linkedUsernames)
                linkedUsers.Add(new LinkedUser { Username = username, Reason = "" });

            linkedUsernames = new List<string>();
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

        public void AddUserLink(string twitchUsername, string reason, IApplicationContext context, DateTime timestamp)
        {
            var existing = linkedUsers.Find(x => string.Equals(x.Username, twitchUsername, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                return;

            linkedUsers.Add(new LinkedUser { Username = twitchUsername, Reason = reason });
            AddAuditTrail(context, nameof(linkedUsernames), linkedUsernames, timestamp);
        }

        public void RemoveUserLink(string twitchUsername, IApplicationContext context, DateTime timestamp)
        {
            var existingEntry = linkedUsers.Find(x => string.Equals(x.Username, twitchUsername, StringComparison.OrdinalIgnoreCase));
            linkedUsers.Remove(existingEntry);

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
                SuspensionSource = SuspensionSource.Listener,
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
                SuspensionSource = SuspensionSource.Listener,
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

        public static Suspension CreateSystemBan(string username, string channelOfOrigin, DateTime timestamp, string systemReason)
        {
            return new Suspension(username, channelOfOrigin, timestamp)
            {
                Audited = true,
                Duration = 0,
                SuspensionType = SuspensionType.Ban,
                SuspensionSource = SuspensionSource.System,
                SystemReason = systemReason,
                ChatMessages = new List<ChatMessage>()
            };
        }
    }
}
