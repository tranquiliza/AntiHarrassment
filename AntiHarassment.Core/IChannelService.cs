using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface IChannelService
    {
        Task<IResult<List<Channel>>> GetChannels(IApplicationContext context);
        Task UpdateChannelListenerState(string channelName, bool shouldListen, IApplicationContext context);
        Task<IResult<Channel>> GetChannel(string channelName, IApplicationContext context);
        Task<IResult<Channel>> AddModeratorToChannel(string channelName, string moderatorTwitchUsername, IApplicationContext context);
        Task<IResult<Channel>> DeleteModeratorFromChannel(string channelName, string moderatorTwitchUsername, IApplicationContext context);
        Task<IResult<List<ChatMessage>>> GetChatLogs(string channelName, DateTime earliestTime, DateTime latestDate, IApplicationContext context);
        Task<IResult<List<string>>> GetChattersForChannel(string channelName, IApplicationContext applicationContext);
        Task<IResult<Channel>> UpdateChannelSystemIsModeratorState(string channelName, bool systemIsModerator, IApplicationContext context);
        Task<IResult<Channel>> RemoveRuleFromChannel(string channelName, Guid ruleId, IApplicationContext context);
        Task<IResult<Channel>> AddRuleToChannel(string channelName, string ruleName, Guid tagId, int bansForTrigger, int timeoutsForTrigger, ChannelRuleAction channelRuleAction, IApplicationContext context);
        Task<IResult<Channel>> UpdateRuleForChannel(string channelName, Guid ruleId, string rulename, int bansForTrigger, int timeoutsForTrigger, ChannelRuleAction channelRuleAction, IApplicationContext context);
        Task InitiateManualRuleCheck(string channelName, string twitchUsername, IApplicationContext applicationContext);
        Task<IResult<List<Channel>>> GetChannelsThatHasNoUser(IApplicationContext context);
    }
}
