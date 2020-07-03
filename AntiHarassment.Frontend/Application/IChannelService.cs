using AntiHarassment.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public interface IChannelService
    {
        Task Initialize();
        List<ChannelModel> Channels { get; }

        event Action OnChange;
    }
}
