using AntiHarassment.Chatlistener.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IChannelRepository
    {
        Task<List<Channel>> GetChannels();
        Task Upsert(Channel channel);
    }
}
