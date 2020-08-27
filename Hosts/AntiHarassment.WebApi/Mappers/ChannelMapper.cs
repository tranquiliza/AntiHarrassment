using AntiHarassment.Contract;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Mappers
{
    public static class ChannelMapper
    {
        public static List<ChannelModel> Map(this List<Channel> channels)
        {
            return channels.Select(Map).ToList();
        }

        public static ChannelModel Map(this Channel channel)
        {
            return new ChannelModel
            {
                ChannelId = channel.ChannelId,
                ChannelName = channel.ChannelName,
                ShouldListen = channel.ShouldListen,
                SystemIsModerator = channel.SystemIsModerator,
                ShouldListenForAutoModdedMessages = channel.ShouldListenForAutoModdedMessages,
                Moderators = channel.Moderators.ToList(),
                ChannelRules = channel.ChannelRules.Select(Map).ToList()
            };
        }

        public static ChannelRuleModel Map(this ChannelRule channelRule)
        {
            return new ChannelRuleModel
            {
                ActionOnTrigger = channelRule.ActionOnTrigger.Map(),
                BansForTrigger = channelRule.BansForTrigger,
                TimeoutsForTrigger = channelRule.TimeoutsForTrigger,
                RuleId = channelRule.RuleId,
                RuleName = channelRule.RuleName,
                Tag = channelRule.Tag.Map()
            };
        }
    }
}
