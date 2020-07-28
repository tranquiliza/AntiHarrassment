using AntiHarassment.Core.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public class Channel : DomainBase
    {
        [JsonProperty]
        public Guid ChannelId { get; private set; }

        [JsonProperty]
        public string ChannelName { get; private set; }

        [JsonProperty]
        public bool ShouldListen { get; private set; }

        [JsonProperty]
        private List<string> moderators { get; set; }

        [JsonIgnore]
        public IReadOnlyList<string> Moderators => moderators.AsReadOnly();

        private Channel() { }

        public Channel(string channelName, bool shouldListen)
        {
            ChannelId = Guid.NewGuid();
            ChannelName = channelName;
            ShouldListen = shouldListen;
            moderators = new List<string>();
        }

        public bool TryAddModerator(string twitchUsername, IApplicationContext context, DateTime timeStamp)
        {
            if (moderators.Contains(twitchUsername))
                return false;

            moderators.Add(twitchUsername);

            AddAuditTrail(context, nameof(moderators), moderators, timeStamp);
            return true;
        }

        public void EnableListening(IApplicationContext context, DateTime timeStamp)
        {
            ShouldListen = true;
            AddAuditTrail(context, nameof(ShouldListen), ShouldListen, timeStamp);
        }

        public void DisableListening(IApplicationContext context, DateTime timeStamp)
        {
            ShouldListen = false;
            AddAuditTrail(context, nameof(ShouldListen), ShouldListen, timeStamp);
        }

        public void RemoveModerator(string twitchUsername, IApplicationContext context, DateTime timeStamp)
        {
            moderators.Remove(twitchUsername);
            AddAuditTrail(context, nameof(moderators), moderators, timeStamp);
        }

        public bool HasModerator(string moderatorUsername)
            => moderators.Any(x => string.Equals(x, moderatorUsername, StringComparison.OrdinalIgnoreCase));
    }
}
