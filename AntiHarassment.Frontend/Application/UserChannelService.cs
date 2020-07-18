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
    public class UserChannelService : IUserChannelService
    {
        private readonly IApiGateway apiGateway;
        private readonly IUserService userService;
        private readonly ChannelsHubSignalRClient channelsHubSignalRClient;

        public ChannelModel Channel { get; private set; }

        public UserChannelService(IApiGateway apiGateway, IUserService userService, ChannelsHubSignalRClient channelsHubSignalRClient)
        {
            this.apiGateway = apiGateway;
            this.userService = userService;
            this.channelsHubSignalRClient = channelsHubSignalRClient;

            channelsHubSignalRClient.ChannelJoined += async (sender, args) => await ChannelsHubSignalRClient_ChannelJoined(sender, args).ConfigureAwait(false);
            channelsHubSignalRClient.ChannelLeft += ChannelsHubSignalRClient_ChannelLeft;
        }

        private async Task ChannelsHubSignalRClient_ChannelJoined(object _, ChannelJoinedEventArgs e)
        {
            if (string.Equals(e.ChannelName, userService.CurrentUserTwitchUsername, StringComparison.OrdinalIgnoreCase))
            {
                if (Channel == null)
                    await FetchChannel().ConfigureAwait(false);

                Channel.ShouldListen = true;
                NotifyStateChanged();
            }
        }

        private void ChannelsHubSignalRClient_ChannelLeft(object _, ChannelLeftEventArgs e)
        {
            if (string.Equals(e.ChannelName, userService.CurrentUserTwitchUsername, StringComparison.OrdinalIgnoreCase))
            {
                Channel.ShouldListen = false;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public event Action OnChange;

        public async Task Initialize()
        {
            if (userService.IsUserLoggedIn)
            {
                await FetchChannel().ConfigureAwait(false);
                await channelsHubSignalRClient.StartAsync().ConfigureAwait(false);
                NotifyStateChanged();
            }
        }

        private async Task FetchChannel()
        {
            var channelParameter = new QueryParam("channelName", userService.CurrentUserTwitchUsername);
            Channel = await apiGateway.Get<ChannelModel>("channels", queryParams: new QueryParam[] { channelParameter }).ConfigureAwait(false);
        }

        public async Task UpdateChannelState(bool shouldListen)
        {
            await apiGateway.Post(new ChannelModel { ChannelName = userService.CurrentUserTwitchUsername, ShouldListen = shouldListen }, "channels").ConfigureAwait(false);
        }

        public async Task AddModerator(string moderatorTwitchUsername)
        {
            var model = new AddModeratorModel { ModeratorTwitchUsername = moderatorTwitchUsername };
            Channel = await apiGateway.Post<ChannelModel, AddModeratorModel>(model, "channels", routeValues: new string[] { userService.CurrentUserTwitchUsername }).ConfigureAwait(false);

            NotifyStateChanged();
        }

        public async Task RemoveModerator(string moderatorTwitchUsername)
        {
            var model = new DeleteModeratorModel { ModeratorTwitchUsername = moderatorTwitchUsername };
            Channel = await apiGateway.Delete<ChannelModel, DeleteModeratorModel>(model, "channels", routeValues: new string[] { userService.CurrentUserTwitchUsername }).ConfigureAwait(false);

            NotifyStateChanged();
        }
    }
}
