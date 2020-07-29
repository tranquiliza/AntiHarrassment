﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace AntiHarassment.Core.Models
{
    public class UserReport
    {
        [JsonProperty]
        public string Username { get; private set; }

        [JsonProperty]
        public List<Suspension> Suspensions { get; private set; }

        [JsonProperty]
        public List<Tag> Tags => Suspensions.SelectMany(x => x.Tags).ToList().GroupBy(x => x.TagName).Select(group => group.First()).ToList();

        [JsonProperty]
        public List<string> BannedFromChannels => Suspensions.GroupBy(x => x.ChannelOfOrigin)
            .Select(groupedSuspensions => groupedSuspensions.FirstOrDefault(y => y.SuspensionType == SuspensionType.Ban)?.ChannelOfOrigin).Where(x => x != null).ToList();

        [JsonProperty]
        public List<string> TimedOutFromChannels => Suspensions.GroupBy(x => x.ChannelOfOrigin)
            .Select(groupedSuspensions => groupedSuspensions.FirstOrDefault(y => y.SuspensionType == SuspensionType.Timeout)?.ChannelOfOrigin).Where(x => x != null).ToList();

        [JsonProperty]
        public List<string> AssociatatedAccounts => Suspensions.SelectMany(x => x.LinkedUsernames).Distinct().ToList();

        public UserReport(string username, List<Suspension> suspensions)
        {
            Username = username;
            Suspensions = suspensions.Where(x => !x.InvalidSuspension && x.Audited).ToList();
        }
    }
}
