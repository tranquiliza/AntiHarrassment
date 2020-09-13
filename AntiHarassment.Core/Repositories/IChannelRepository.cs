using AntiHarassment.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface IChannelRepository
    {
        Task<List<Channel>> GetChannels();
        Task Upsert(Channel channel);
        Task<Channel> GetChannel(string twitchUsername);
    }
}
