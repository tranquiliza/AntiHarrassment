using AntiHarassment.Contract;
using AntiHarassment.Frontend.Infrastructure;
using AntiHarassment.SignalR.Contract;
using AntiHarassment.SignalR.Contract.EventArgs;
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
        private readonly SuspensionsHubSignalRClient suspensionsHubSignalRClient;

        public ChannelStatisticsService(IApiGateway apiGateway, IUserService userService, SuspensionsHubSignalRClient suspensionsHubSignalRClient)
        {
            this.apiGateway = apiGateway;
            this.userService = userService;
            this.suspensionsHubSignalRClient = suspensionsHubSignalRClient;

            suspensionsHubSignalRClient.OnNewSuspension += async (sender, args) => await SuspensionsHubSignalRClient_OnNewSuspension(sender, args).ConfigureAwait(false);
            suspensionsHubSignalRClient.OnSuspensionUpdated += async (sender, args) => await SuspensionsHubSignalRClient_OnSuspensionUpdated(sender, args).ConfigureAwait(false);
        }

        private async Task SuspensionsHubSignalRClient_OnSuspensionUpdated(object _, SuspensionUpdatedEventArgs e)
        {
            if (string.Equals(e.ChannelOfOrigin, CurrentlySelectedChannel, StringComparison.OrdinalIgnoreCase))
            {
                UserRulesExceededModels = await apiGateway.Get<List<UserRulesExceededModel>>("channels", routeValues: new string[] { CurrentlySelectedChannel, "channelRules", "exceeded" }).ConfigureAwait(false);
                NotifyStateChanged();
            }
        }

        private async Task SuspensionsHubSignalRClient_OnNewSuspension(object _, NewSuspensionEventArgs e)
        {
            if (string.Equals(e.ChannelOfOrigin, CurrentlySelectedChannel, StringComparison.OrdinalIgnoreCase))
            {
                UserRulesExceededModels = await apiGateway.Get<List<UserRulesExceededModel>>("channels", routeValues: new string[] { CurrentlySelectedChannel, "channelRules", "exceeded" }).ConfigureAwait(false);
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
        public event Action OnChange;

        public async Task Initialize()
        {
            CurrentlySelectedChannel = userService.CurrentUserTwitchUsername;
            ChannelReportModel = await apiGateway.Get<ChannelReportModel>("channels", routeValues: new string[] { userService.CurrentUserTwitchUsername, "report" }).ConfigureAwait(false);
            UserRulesExceededModels = await apiGateway.Get<List<UserRulesExceededModel>>("channels", routeValues: new string[] { userService.CurrentUserTwitchUsername, "channelRules", "exceeded" }).ConfigureAwait(false);
            await suspensionsHubSignalRClient.StartAsync().ConfigureAwait(false);
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

        public async Task ManuallyInvokeRuleCheck(string twitchUsername)
        {
            var requestModel = new ManuallyRunRuleCheckModel { TwitchUsername = twitchUsername };
            await apiGateway.Post(requestModel, "channels", routeValues: new string[] { CurrentlySelectedChannel, "users", "ruleCheck" }).ConfigureAwait(false);
        }
    }
}
