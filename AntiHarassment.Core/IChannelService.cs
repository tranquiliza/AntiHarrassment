using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface IChannelService
    {
        Task<IResult<List<Channel>>> GetChannels(IApplicationContext context);
        Task UpdateChannel(string channelName, bool shouldListen, IApplicationContext context);
        Task<IResult<Channel>> GetChannel(string channelName, IApplicationContext context);
        Task<IResult<Channel>> AddModeratorToChannel(string channelName, string moderatorTwitchUsername, IApplicationContext context);
        Task<IResult<Channel>> DeleteModeratorFromChannel(string channelName, string moderatorTwitchUsername, IApplicationContext context);
        Task<IResult<List<ChatMessage>>> GetChatLogs(string channelName, DateTime earliestTime, DateTime latestDate, IApplicationContext context);
    }
}
