using AntiHarassment.Contract;
using AntiHarassment.Core.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AntiHarassment.WebApi.Mappers
{
    public static class ChannelRulesMapper
    {
        [SuppressMessage("General", "RCS1079:Throwing of new NotImplementedException.", Justification = "Can't implement future features before we're in the future.")]
        public static ChannelRuleAction Map(this ChannelRuleActionModel model)
        {
            return model switch
            {
                ChannelRuleActionModel.None => ChannelRuleAction.None,
                ChannelRuleActionModel.Ban => ChannelRuleAction.Ban,
                ChannelRuleActionModel.NotifyWebsite => ChannelRuleAction.NotifyWebsite,
                ChannelRuleActionModel.NotifyDiscord => ChannelRuleAction.NotifyDiscord,
                _ => throw new NotImplementedException("No mapping for this value"),
            };
        }

        [SuppressMessage("General", "RCS1079:Throwing of new NotImplementedException.", Justification = "Can't implement future features before we're in the future.")]
        public static ChannelRuleActionModel Map(this ChannelRuleAction action)
        {
            return action switch
            {
                ChannelRuleAction.None => ChannelRuleActionModel.None,
                ChannelRuleAction.Ban => ChannelRuleActionModel.Ban,
                ChannelRuleAction.NotifyWebsite => ChannelRuleActionModel.NotifyWebsite,
                ChannelRuleAction.NotifyDiscord => ChannelRuleActionModel.NotifyDiscord,
                _ => throw new NotImplementedException("No mapping for this value"),
            };
        }
    }
}
