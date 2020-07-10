using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface IChannelService
    {
        Task<List<Channel>> GetChannels();
        Task UpdateChannel(string channelName, bool shouldListen);
    }
}
