using AntiHarassment.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public interface IAdminChannelService
    {
        Task Initialize();
        Task UpdateChannel(ChannelModel channelModel);
        Task UpdateChannelSystemIsModerator(string channelName, bool systemIsModerator);

        List<ChannelModel> Channels { get; }

        event Action OnChange;
    }
}
