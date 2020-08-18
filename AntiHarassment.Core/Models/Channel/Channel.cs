using AntiHarassment.Core.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public bool ShouldListenForAutoModdedMessages { get; private set; }

        [JsonProperty]
        public bool SystemIsModerator { get; private set; }

        [JsonProperty]
        private List<string> moderators { get; set; }

        [JsonIgnore]
        public IReadOnlyList<string> Moderators => moderators.AsReadOnly();

        [JsonProperty]
        private List<ChannelRule> channelRules { get; set; } = new List<ChannelRule>();

        [JsonIgnore]
        public IReadOnlyList<ChannelRule> ChannelRules => channelRules.AsReadOnly();

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

        public void UpdateSystemModerationStatus(bool newStatus, IApplicationContext context, DateTime timeStamp)
        {
            SystemIsModerator = newStatus;

            AddAuditTrail(context, nameof(SystemIsModerator), newStatus, timeStamp);
        }

        public void EnableAutoModdedMessageListening(IApplicationContext context, DateTime timeStamp)
        {
            ShouldListenForAutoModdedMessages = true;

            AddAuditTrail(context, nameof(ShouldListenForAutoModdedMessages), true, timeStamp);
        }

        public void DisableAutoModdedMessageListening(IApplicationContext context, DateTime timeStamp)
        {
            ShouldListenForAutoModdedMessages = false;

            AddAuditTrail(context, nameof(ShouldListenForAutoModdedMessages), false, timeStamp);
        }

        public bool HasModerator(string moderatorUsername)
            => moderators.Any(x => string.Equals(x, moderatorUsername, StringComparison.OrdinalIgnoreCase));

        public void RemoveRule(Guid ruleId)
        {
            if (channelRules == null)
                channelRules = new List<ChannelRule>();

            var lookup = channelRules.Find(x => x.RuleId == ruleId);
            channelRules.Remove(lookup);
        }

        public void AddRule(string ruleName, Tag tag, int bansForTrigger, int timeoutsForTrigger, ChannelRuleAction channelRuleAction)
        {
            if (channelRules == null)
                channelRules = new List<ChannelRule>();

            // we might need to add a check if there is an equivelant rule? (No need to have two that does the exact same)
            var newRule = new ChannelRule(ruleName, tag, bansForTrigger, timeoutsForTrigger, channelRuleAction);
            channelRules.Add(newRule);
        }

        internal void UpdateRule(Guid ruleId, string rulename, int bansForTrigger, int timeoutsForTrigger, ChannelRuleAction channelRuleAction)
        {
            if (channelRules == null)
                channelRules = new List<ChannelRule>();

            var existingRule = channelRules.Find(x => x.RuleId == ruleId);
            if (existingRule != null)
            {
                channelRules.Remove(existingRule);
                existingRule.Update(rulename, bansForTrigger, timeoutsForTrigger, channelRuleAction);
                channelRules.Add(existingRule);
            }
        }
    }
}
