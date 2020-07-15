using AntiHarassment.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public interface IUserChannelService
    {
        event Action OnChange;
        ChannelModel Channel { get; }

        Task Initialize();
        Task AddModerator(string moderatorTwitchUsername);
        Task RemoveModerator(string moderatorTwitchUsername);
    }
}
