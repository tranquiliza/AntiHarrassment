using AntiHarassment.Contract;
using AntiHarassment.Frontend.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public class ChannelStatisticsService : IChannelStatisticsService
    {
        public ChannelReportModel ChannelReportModel { get; set; }

        public List<UserRulesExceededModel> UserRulesExceededModels { get; set; }

        public string CurrentlySelectedChannel { get; set; }

        private readonly IApiGateway apiGateway;
        private readonly IUserService userService;

        public ChannelStatisticsService(IApiGateway apiGateway, IUserService userService)
        {
            this.apiGateway = apiGateway;
            this.userService = userService;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
        public event Action OnChange;

        public async Task Initialize()
        {
            CurrentlySelectedChannel = userService.CurrentUserTwitchUsername;
            ChannelReportModel = await apiGateway.Get<ChannelReportModel>("channels", routeValues: new string[] { userService.CurrentUserTwitchUsername, "report" }).ConfigureAwait(false);
            UserRulesExceededModels = await apiGateway.Get<List<UserRulesExceededModel>>("channels", routeValues: new string[] { userService.CurrentUserTwitchUsername, "channelRules", "exceeded" }).ConfigureAwait(false);
            NotifyStateChanged();
        }

        public async Task ChangeChannel(string selectedChannel)
        {
            CurrentlySelectedChannel = selectedChannel;

            ChannelReportModel = null;
            UserRulesExceededModels = null;
            ChannelReportModel = await apiGateway.Get<ChannelReportModel>("channels", routeValues: new string[] { CurrentlySelectedChannel, "report" }).ConfigureAwait(false);
            UserRulesExceededModels = await apiGateway.Get<List<UserRulesExceededModel>>("channels", routeValues: new string[] { CurrentlySelectedChannel, "channelRules", "exceeded" }).ConfigureAwait(false);
            NotifyStateChanged();
        }
    }
}
