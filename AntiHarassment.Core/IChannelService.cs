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
        Task UpdateChannel(string channelName, bool shouldListen);
        Task<IResult<Channel>> GetChannel(string channelName, IApplicationContext context);
    }
}
