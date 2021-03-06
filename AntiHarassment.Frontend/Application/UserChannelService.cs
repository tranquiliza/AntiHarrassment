﻿using AntiHarassment.Contract;
using AntiHarassment.Frontend.Infrastructure;
using AntiHarassment.SignalR.Contract;
using AntiHarassment.SignalR.Contract.EventArgs;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public class UserChannelService : IUserChannelService
    {
        private readonly IApiGateway apiGateway;
        private readonly IUserService userService;
        private readonly ChannelsHubSignalRClient channelsHubSignalRClient;
        private readonly IJSRuntime jSRuntime;

        private string CurrentlySelectedChannelName = "";

        public ChannelModel Channel { get; private set; }

        public UserChannelService(IApiGateway apiGateway, IUserService userService, IJSRuntime jSRuntime, ChannelsHubSignalRClient channelsHubSignalRClient)
        {
            this.apiGateway = apiGateway;
            this.userService = userService;
            this.jSRuntime = jSRuntime;
            this.channelsHubSignalRClient = channelsHubSignalRClient;

            channelsHubSignalRClient.ChannelJoined += async (sender, args) => await ChannelsHubSignalRClient_ChannelJoined(sender, args).ConfigureAwait(false);
            channelsHubSignalRClient.ChannelLeft += ChannelsHubSignalRClient_ChannelLeft;
            channelsHubSignalRClient.AutoModListenerEnabled += ChannelsHubSignalRClient_AutoModListenerEnabled;
            channelsHubSignalRClient.AutoModListenerDisabled += ChannelsHubSignalRClient_AutoModListenerDisabled;
        }

        private void ChannelsHubSignalRClient_AutoModListenerDisabled(object sender, AutoModListenerDisabledEventArgs e)
        {
            if (string.Equals(e.ChannelName, CurrentlySelectedChannelName, StringComparison.OrdinalIgnoreCase))
            {
                Channel.ShouldListenForAutoModdedMessages = false;
                NotifyStateChanged();
            }
        }

        private void ChannelsHubSignalRClient_AutoModListenerEnabled(object sender, AutoModListenerEnabledEventArgs e)
        {
            if (string.Equals(e.ChannelName, CurrentlySelectedChannelName, StringComparison.OrdinalIgnoreCase))
            {
                Channel.ShouldListenForAutoModdedMessages = true;
                NotifyStateChanged();
            }
        }

        private async Task ChannelsHubSignalRClient_ChannelJoined(object _, ChannelJoinedEventArgs e)
        {
            if (string.Equals(e.ChannelName, CurrentlySelectedChannelName, StringComparison.OrdinalIgnoreCase))
            {
                if (Channel == null)
                    await FetchChannel().ConfigureAwait(false);

                Channel.ShouldListen = true;
                NotifyStateChanged();
            }
        }

        private void ChannelsHubSignalRClient_ChannelLeft(object _, ChannelLeftEventArgs e)
        {
            if (string.Equals(e.ChannelName, CurrentlySelectedChannelName, StringComparison.OrdinalIgnoreCase))
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
                CurrentlySelectedChannelName = userService.CurrentUserTwitchUsername;
                await FetchChannel().ConfigureAwait(false);
                await channelsHubSignalRClient.StartAsync().ConfigureAwait(false);
                NotifyStateChanged();
            }
        }

        private async Task FetchChannel()
        {
            var channelParameter = new QueryParam("channelName", CurrentlySelectedChannelName);
            Channel = await apiGateway.Get<ChannelModel>("channels", queryParams: new QueryParam[] { channelParameter }).ConfigureAwait(false);
        }

        public async Task ChangeChannel(string channelName)
        {
            Channel = null;
            CurrentlySelectedChannelName = channelName;
            await FetchChannel().ConfigureAwait(false);

            NotifyStateChanged();
        }

        public async Task UpdateChannelState(bool shouldListen)
        {
            await apiGateway.Post(new ChannelModel { ChannelName = CurrentlySelectedChannelName, ShouldListen = shouldListen }, "channels").ConfigureAwait(false);
        }

        public async Task AddModerator(string moderatorTwitchUsername)
        {
            var model = new AddModeratorModel { ModeratorTwitchUsername = moderatorTwitchUsername };
            Channel = await apiGateway.Post<ChannelModel, AddModeratorModel>(model, "channels", routeValues: new string[] { CurrentlySelectedChannelName, "moderators" }).ConfigureAwait(false);

            NotifyStateChanged();
        }

        public async Task RemoveModerator(string moderatorTwitchUsername)
        {
            var model = new DeleteModeratorModel { ModeratorTwitchUsername = moderatorTwitchUsername };
            Channel = await apiGateway.Delete<ChannelModel, DeleteModeratorModel>(model, "channels", routeValues: new string[] { CurrentlySelectedChannelName, "moderators" }).ConfigureAwait(false);

            NotifyStateChanged();
        }

        public async Task CreateNewChannelRule(AddChannelRuleModel model)
        {
            Channel = await apiGateway.Post<ChannelModel, AddChannelRuleModel>(model, "channels", routeValues: new string[] { CurrentlySelectedChannelName, "channelRules" }).ConfigureAwait(false);

            NotifyStateChanged();
        }

        public async Task UpdateChannelRule(UpdateChannelRuleModel model, Guid ruleId)
        {
            Channel = await apiGateway.Post<ChannelModel, UpdateChannelRuleModel>(model, "channels", routeValues: new string[] { CurrentlySelectedChannelName, "channelRules", ruleId.ToString() }).ConfigureAwait(false);

            NotifyStateChanged();
        }

        public async Task RemoveChannelRule(Guid ruleId)
        {
            var model = new DeleteChannelRuleModel { RuleId = ruleId };
            Channel = await apiGateway.Delete<ChannelModel, DeleteChannelRuleModel>(model, "channels", routeValues: new string[] { CurrentlySelectedChannelName, "channelRules" }).ConfigureAwait(false);

            NotifyStateChanged();
        }

        public async Task UpdateSystemModeratorState(bool systemIsModerator)
        {
            var model = new UpdateSystemIsModeratorStatusModel { SystemIsModerator = systemIsModerator };

            Channel = await apiGateway.Post<ChannelModel, UpdateSystemIsModeratorStatusModel>(model, "channels", routeValues: new string[] { CurrentlySelectedChannelName, "systemIsModerator" }).ConfigureAwait(false);

            NotifyStateChanged();
        }

        public async Task<List<ChatMessageModel>> DownloadChatLogsForPreview(DateTime earliestTime, DateTime latestTime)
        {
            var chatlogs = await DownloadMessages(earliestTime, latestTime).ConfigureAwait(false);
            return chatlogs ?? new List<ChatMessageModel>();
        }

        private const string dateFormat = "yyyy-MM-dd HH:mm:ss";

        private async Task<List<ChatMessageModel>> DownloadMessages(DateTime earliestTime, DateTime latestTime)
        {
            var currentUniversalTime = DateTime.UtcNow;
            var currentLocalTime = DateTime.Now;
            var difference = currentUniversalTime - currentLocalTime;

            var correctedEarlyTime = earliestTime.Add(difference);
            var correctedLatestTime = latestTime.Add(difference);

            var earliestParam = new QueryParam("earliestTime", correctedEarlyTime.ToString(dateFormat));
            var latestParam = new QueryParam("latestTime", correctedLatestTime.ToString(dateFormat));

            return await apiGateway.Get<List<ChatMessageModel>>(
                "channels",
                routeValues: new string[] { Channel.ChannelName, "chatlogs" },
                queryParams: new QueryParam[] { earliestParam, latestParam }).ConfigureAwait(false);
        }

        public async Task DownloadChatLog(DateTime earliestTime, DateTime latestTime, bool downloadPlain)
        {
            var chatLogs = await DownloadMessages(earliestTime, latestTime).ConfigureAwait(false);
            if (chatLogs == null)
                return;

            var fileName = $"CHATLOG_{earliestTime.ToString(dateFormat)}_{latestTime.ToString(dateFormat)}";

            if (downloadPlain)
                await DownloadAsCsv(chatLogs, fileName).ConfigureAwait(false);
            else
                await DownloadAsJson(chatLogs, fileName).ConfigureAwait(false);
        }

        private async Task DownloadAsJson(List<ChatMessageModel> chatLogs, string fileName)
        {
            await jSRuntime.InvokeVoidAsync("DownloadAsFile", Serialization.SerializePretty(chatLogs), fileName + ".json", "application/json");
        }

        private async Task DownloadAsCsv(List<ChatMessageModel> chatLogs, string fileName)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Timestamp;AutoModded;Username;Message;");
            foreach (var message in chatLogs)
                builder.AppendLine(ConvertToLine(message));

            await jSRuntime.InvokeVoidAsync("DownloadAsFile", builder.ToString(), fileName + ".txt", "text/plain");
        }

        private string ConvertToLine(ChatMessageModel model)
        {
            return $"{model.Timestamp};{model.AutoModded};{model.Username};{model.Message};";
        }
    }
}
