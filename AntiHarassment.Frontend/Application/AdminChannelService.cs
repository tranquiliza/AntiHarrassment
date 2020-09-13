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
    public class AdminChannelService : IAdminChannelService
    {
        public List<ChannelModel> Channels { get; private set; }

        public List<ChannelModel> ChannelsWithoutUser { get; private set; }

        private readonly IApiGateway apiGateway;
        private readonly IUserService userService;
        private readonly ChannelsHubSignalRClient channelsHubSignalRClient;

        public AdminChannelService(IApiGateway apiGateway, IUserService userService, ChannelsHubSignalRClient channelsHubSignalRClient)
        {
            this.apiGateway = apiGateway;
            this.userService = userService;
            this.channelsHubSignalRClient = channelsHubSignalRClient;

            channelsHubSignalRClient.ChannelJoined += async (sender, args) => await ChannelsHubSignalRClient_ChannelJoined(sender, args).ConfigureAwait(false);
            channelsHubSignalRClient.ChannelLeft += ChannelsHubSignalRClient_ChannelLeft;
            channelsHubSignalRClient.AutoModListenerDisabled += ChannelsHubSignalRClient_AutoModListenerDisabled;
            channelsHubSignalRClient.AutoModListenerEnabled += ChannelsHubSignalRClient_AutoModListenerEnabled;

            userService.OnChange += UserService_OnChange;
        }

        private void ChannelsHubSignalRClient_AutoModListenerEnabled(object sender, AutoModListenerEnabledEventArgs e)
        {
            UpdateAutoModListenerForChannels(e.ChannelName, true);

            NotifyStateChanged();
        }

        private void ChannelsHubSignalRClient_AutoModListenerDisabled(object sender, AutoModListenerDisabledEventArgs e)
        {
            UpdateAutoModListenerForChannels(e.ChannelName, false);

            NotifyStateChanged();
        }

        private async Task ChannelsHubSignalRClient_ChannelJoined(object _, ChannelJoinedEventArgs e)
        {
            if (!Channels.Any(x => x.ChannelName == e.ChannelName))
            {
                await FetchChannels().ConfigureAwait(false);
            }
            else
            {
                UpdateConnectedStateForChannels(e.ChannelName, true);
            }

            NotifyStateChanged();
        }

        private void ChannelsHubSignalRClient_ChannelLeft(object _, ChannelLeftEventArgs e)
        {
            UpdateConnectedStateForChannels(e.ChannelName, false);

            NotifyStateChanged();
        }

        private void UpdateAutoModListenerForChannels(string channelName, bool autoModIsListening)
        {
            var channel = Channels.Find(x => x.ChannelName == channelName);
            if (channel != null)
                channel.ShouldListenForAutoModdedMessages = autoModIsListening;

            var channelWithoutUser = ChannelsWithoutUser.Find(x => x.ChannelName == channelName);
            if (channelWithoutUser != null)
                channelWithoutUser.ShouldListenForAutoModdedMessages = autoModIsListening;
        }

        private void UpdateConnectedStateForChannels(string channelName, bool shouldListen)
        {
            var channel = Channels.Find(x => x.ChannelName == channelName);
            if (channel != null)
                channel.ShouldListen = shouldListen;

            var channelWithoutUser = ChannelsWithoutUser.Find(x => x.ChannelName == channelName);
            if (channelWithoutUser != null)
                channelWithoutUser.ShouldListen = shouldListen;
        }

        private void UserService_OnChange()
        {
            if (!userService.IsUserLoggedIn)
            {
                Channels = null;
                ChannelsWithoutUser = null;
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public event Action OnChange;

        private bool isInitialized = false;

        public async Task Initialize()
        {
            if (userService.IsUserAdmin && !isInitialized)
            {
                await channelsHubSignalRClient.StartAsync().ConfigureAwait(false);
                await FetchChannels().ConfigureAwait(false);

                isInitialized = true;
                NotifyStateChanged();
            }
        }

        public async Task UpdateChannelSystemIsModerator(string channelName, bool systemIsModerator)
        {
            var model = new UpdateSystemIsModeratorStatusModel { SystemIsModerator = systemIsModerator };

            var channel = await apiGateway.Post<ChannelModel, UpdateSystemIsModeratorStatusModel>(model, "channels", routeValues: new string[] { channelName, "systemIsModerator" }).ConfigureAwait(false);

            var existingChannel = Channels.Find(x => string.Equals(x.ChannelName, channelName, StringComparison.OrdinalIgnoreCase));
            if (existingChannel != null)
            {
                Channels.Remove(existingChannel);
                Channels.Add(channel);
            }

            var existingChannelWithoutUser = ChannelsWithoutUser.Find(x => string.Equals(x.ChannelName, channelName, StringComparison.OrdinalIgnoreCase));
            if (existingChannelWithoutUser != null)
            {
                ChannelsWithoutUser.Remove(existingChannelWithoutUser);
                ChannelsWithoutUser.Add(channel);
            }

            NotifyStateChanged();
        }

        private async Task FetchChannels()
        {
            Channels = null;
            ChannelsWithoutUser = null;

            Channels = await apiGateway.Get<List<ChannelModel>>("Channels").ConfigureAwait(false);
            ChannelsWithoutUser = await apiGateway.Get<List<ChannelModel>>("Channels", routeValues: new string[] { "noUser" }).ConfigureAwait(false);
        }

        public async Task UpdateChannel(ChannelModel channelModel)
        {
            await apiGateway.Post(channelModel, "Channels").ConfigureAwait(false);
        }
    }
}
