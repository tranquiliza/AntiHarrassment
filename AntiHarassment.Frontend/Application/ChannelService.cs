using AntiHarassment.Contract;
using AntiHarassment.Frontend.Infrastructure;
using AntiHarassment.SignalR.Contract;
using AntiHarassment.SignalR.Contract.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public class ChannelService : IChannelService
    {
        public List<ChannelModel> Channels { get; private set; }

        private readonly IApiGateway apiGateway;
        private readonly IUserService userService;
        private readonly ChannelsHubSignalRClient channelsHubSignalRClient;

        public ChannelService(IApiGateway apiGateway, IUserService userService, ChannelsHubSignalRClient channelsHubSignalRClient)
        {
            this.apiGateway = apiGateway;
            this.userService = userService;
            this.channelsHubSignalRClient = channelsHubSignalRClient;


            channelsHubSignalRClient.ChannelJoined += async (sender, args) => await ChannelsHubSignalRClient_ChannelJoined(sender, args).ConfigureAwait(false);
            channelsHubSignalRClient.ChannelLeft += ChannelsHubSignalRClient_ChannelLeft;

            userService.OnChange += UserService_OnChange;
        }

        private async Task ChannelsHubSignalRClient_ChannelJoined(object _, ChannelJoinedEventArgs e)
        {
            if (!Channels.Any(x => x.ChannelName == e.ChannelName))
            {
                await FetchChannels().ConfigureAwait(false);
            }
            else
            {
                var channel = Channels.Find(x => x.ChannelName == e.ChannelName);
                if (channel != null)
                    channel.ShouldListen = true;
            }

            NotifyStateChanged();
        }

        private void ChannelsHubSignalRClient_ChannelLeft(object _, ChannelLeftEventArgs e)
        {
            var channel = Channels.Find(x => x.ChannelName == e.ChannelName);
            if (channel != null)
                channel.ShouldListen = false;

            NotifyStateChanged();
        }

        private void UserService_OnChange()
        {
            if (!userService.IsUserLoggedIn)
                Channels = null;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public event Action OnChange;

        public async Task Initialize()
        {
            if (userService.IsUserAdmin)
            {
                await channelsHubSignalRClient.StartAsync().ConfigureAwait(false);
                await FetchChannels().ConfigureAwait(false);
                NotifyStateChanged();
            }
        }

        private async Task FetchChannels()
        {
            Channels = await apiGateway.Get<List<ChannelModel>>("Channels").ConfigureAwait(false);
        }

        public async Task UpdateChannel(ChannelModel channelModel)
        {
            await apiGateway.Post(channelModel, "Channels").ConfigureAwait(false);
        }
    }
}
