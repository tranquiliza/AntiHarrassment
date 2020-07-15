using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public class Channel
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

        [Obsolete("Serialization Only", true)]
        public Channel()
        {
        }

        public Channel(string channelName, bool shouldListen)
        {
            ChannelId = Guid.NewGuid();
            ChannelName = channelName;
            ShouldListen = shouldListen;
            moderators = new List<string>();
        }

        public bool TryAddModerator(string twitchUsername)
        {
            if (moderators.Contains(twitchUsername))
                return false;

            moderators.Add(twitchUsername);

            return true;
        }

        public void EnableListening()
        {
            ShouldListen = true;
        }

        public void DisableListening()
        {
            ShouldListen = false;
        }

        public void RemoveModerator(string twitchUsername)
        {
            moderators.Remove(twitchUsername);
        }

        public bool HasModerator(string moderatorUsername)
            => moderators.Any(x => string.Equals(x, moderatorUsername, StringComparison.OrdinalIgnoreCase));
    }
}
