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
        Task UpdateChannelState(bool shouldListen);
        Task DownloadChatLog(DateTime earliestTime, DateTime latestTime, bool downloadPlain);
        Task ChangeChannel(string channelName);
        Task CreateNewChannelRule(AddChannelRuleModel model);
        Task RemoveChannelRule(Guid ruleId);
        Task UpdateChannelRule(UpdateChannelRuleModel model, Guid ruleId);
        Task UpdateSystemModeratorState(bool systemIsModerator);
    }
}
