using AntiHarassment.Contract;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Mappers
{
    public static class ChannelRulesMapper
    {
        public static ChannelRuleAction Map(this ChannelRuleActionModel model)
        {
            return model switch
            {
                ChannelRuleActionModel.None => ChannelRuleAction.None,
                ChannelRuleActionModel.Ban => ChannelRuleAction.Ban,
                _ => throw new NotImplementedException("No mapping for this value"),
            };
        }

        public static ChannelRuleActionModel Map(this ChannelRuleAction action)
        {
            return action switch
            {
                ChannelRuleAction.None => ChannelRuleActionModel.None,
                ChannelRuleAction.Ban => ChannelRuleActionModel.Ban,
                _ => throw new NotImplementedException("No mapping for this value"),
            };
        }
    }
}
