using AntiHarassment.Contract;
using AntiHarassment.Frontend.Infrastructure;
using AntiHarassment.SignalR.Contract;
using AntiHarassment.SignalR.Contract.EventArgs;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public class SuspensionService : ISuspensionService, IDisposable
    {
        private List<string> usersFromChannel { get; set; }
        public List<ChannelModel> Channels { get; private set; }
        public List<string> UsersFromChannel
        {
            get
            {
                return usersFromChannel?.Where(
                    username => username.StartsWith(string.IsNullOrEmpty(CurrentSearchTerm) ? "_" : CurrentSearchTerm, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(username, CurrentlySelectedSuspension?.Username)
                    && CurrentlySelectedSuspension?.LinkedUsers.Any(userLink => string.Equals(username, userLink.Username, StringComparison.OrdinalIgnoreCase)) == false
                ).ToList();
            }
        }

        public string UserLinkReason { get; set; }
        public string CurrentSearchTerm { get; set; }
        public SuspensionModel CurrentlySelectedSuspension { get; set; }

        public string CurrentInvalidationReason { get; set; }
        public SuspensionModel CurrentlySelectedSuspensionForInvalidation { get; set; }

        public string CurrentlySelectedChannel { get; private set; }
        public List<SuspensionModel> Suspensions { get; private set; }
        public SuspensionModel CurrentlySelectedSuspensionForImages { get; private set; }
        public List<DateTime> DatesWithUnauditedSuspensions { get; set; }
        public DateTime SelectedDate { get; set; } = DateTime.Now;

        private readonly IApiGateway apiGateway;
        private readonly IUserService userService;
        private readonly SuspensionsHubSignalRClient suspensionsHub;
        private readonly IJSRuntime jSRuntime;

        public SuspensionService(IApiGateway apiGateway, IUserService userService, IJSRuntime jSRuntime, SuspensionsHubSignalRClient suspensionsHub)
        {
            this.apiGateway = apiGateway;
            this.userService = userService;
            this.suspensionsHub = suspensionsHub;

            suspensionsHub.OnNewSuspension += async (sender, args) => await SuspensionsHub_OnNewSuspension(sender, args);
            suspensionsHub.OnSuspensionUpdated += async (sender, args) => await SuspensionsHub_SuspensionUpdated(sender, args);
            this.jSRuntime = jSRuntime;
        }

        private async Task SuspensionsHub_SuspensionUpdated(object _, SuspensionUpdatedEventArgs args)
        {
            if (!string.Equals(CurrentlySelectedChannel, args.ChannelOfOrigin, StringComparison.OrdinalIgnoreCase))
                return;

            var qParam = new QueryParam("suspensionId", args.SuspensionId.ToString());
            var updatedSuspension = await apiGateway.Get<SuspensionModel>("suspensions", queryParams: qParam).ConfigureAwait(false);
            if (updatedSuspension == null)
                return;

            var existingSuspension = Suspensions.Find(x => x.SuspensionId == args.SuspensionId);
            if (existingSuspension != null)
                Suspensions.Remove(existingSuspension);

            if (updatedSuspension.Audited)
                await FetchDaysWithUnauditedSuspensions(CurrentlySelectedChannel).ConfigureAwait(false);

            if (updatedSuspension.Timestamp.ToLocalTime().Date == SelectedDate.Date)
                Suspensions.Add(updatedSuspension);

            NotifyStateChanged();
        }

        private async Task SuspensionsHub_OnNewSuspension(object _, NewSuspensionEventArgs args)
        {
            if (!string.Equals(CurrentlySelectedChannel, args.ChannelOfOrigin, StringComparison.OrdinalIgnoreCase))
                return;

            var qParam = new QueryParam("suspensionId", args.SuspensionId.ToString());
            var newSuspension = await apiGateway.Get<SuspensionModel>("suspensions", queryParams: qParam).ConfigureAwait(false);
            UpdateState(newSuspension);
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public event Action OnChange;

        public async Task Initialize()
        {
            Channels = await apiGateway.Get<List<ChannelModel>>("Channels").ConfigureAwait(false);
            if (Channels == null)
                return;

            await suspensionsHub.StartAsync().ConfigureAwait(false);

            if (!Channels.Any(x => string.Equals(x.ChannelName, userService.CurrentUserTwitchUsername, StringComparison.OrdinalIgnoreCase)))
            {
                if (Channels.Count > 0)
                {
                    await FetchSuspensionForChannel(Channels[0].ChannelName).ConfigureAwait(false);
                    await FetchDaysWithUnauditedSuspensions(Channels[0].ChannelName).ConfigureAwait(false);
                    await FetchSeenUsersForChannel(Channels[0].ChannelName).ConfigureAwait(false);
                }
            }
            else
            {
                await FetchSuspensionForChannel(userService.CurrentUserTwitchUsername).ConfigureAwait(false);
                await FetchDaysWithUnauditedSuspensions(userService.CurrentUserTwitchUsername).ConfigureAwait(false);
                await FetchSeenUsersForChannel(userService.CurrentUserTwitchUsername).ConfigureAwait(false);
            }
        }

        public async Task FetchDaysWithUnauditedSuspensions(string channelName)
        {
            DatesWithUnauditedSuspensions = null;

            var result = await apiGateway.Get<List<DateTime>>("suspensions", routeValues: new string[] { channelName, "unauditedDates" }).ConfigureAwait(false);

            DatesWithUnauditedSuspensions = result ?? new List<DateTime>();

            if (DatesWithUnauditedSuspensions.Count > 0)
                NotifyStateChanged();
        }

        public void SetCurrentlySelectedSuspensionForImages(SuspensionModel suspension)
        {
            CurrentlySelectedSuspensionForImages = suspension;
            NotifyStateChanged();
        }

        public async Task FetchSuspensionForChannel(string channelName)
        {
            Suspensions = null;

            var result = await apiGateway.Get<List<SuspensionModel>>("suspensions", routeValues: new string[] { channelName }).ConfigureAwait(false);
            Suspensions = result ?? new List<SuspensionModel>();

            CurrentlySelectedChannel = channelName;
        }

        public async Task FetchSeenUsersForChannel(string channelName)
        {
            usersFromChannel = null;
            var result = await apiGateway.Get<List<string>>("channels", routeValues: new string[] { channelName, "users" }).ConfigureAwait(false);
            usersFromChannel = result ?? new List<string>();
        }

        public async Task AddUserLinkToSuspension(Guid suspensionId, string twitchUsername, string reasonForLink)
        {
            var result = await apiGateway.Post<SuspensionModel, AddUserLinkToSuspensionModel>(
                new AddUserLinkToSuspensionModel { Username = twitchUsername, LinkUserReason = reasonForLink },
                "suspensions",
                routeValues: new string[] { suspensionId.ToString(), "userlink" }).ConfigureAwait(false);

            UpdateState(result);
        }

        public async Task RemoveUserLinkFromSuspension(Guid suspensionId, string twitchUsername)
        {
            var result = await apiGateway.Delete<SuspensionModel, DeleteUserlinkFromSuspensionModel>(
                new DeleteUserlinkFromSuspensionModel { Username = twitchUsername },
                "suspensions",
                routeValues: new string[] { suspensionId.ToString(), "userlink" }).ConfigureAwait(false);

            UpdateState(result);
        }

        public async Task UpdateSuspensionValidity(Guid suspensionId, bool invalidate, string invalidationReason = "")
        {
            try
            {
                var result = await apiGateway.Post<SuspensionModel, MarkSuspensionValidityModel>(
                    new MarkSuspensionValidityModel { Invalidate = invalidate, InvalidationReason = invalidationReason },
                    "suspensions",
                    routeValues: new string[] { suspensionId.ToString(), "validity" }).ConfigureAwait(false);

                CurrentInvalidationReason = "";

                UpdateState(result);
            }
            catch (Exception ex)
            {
                await jSRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }

        public async Task UpdateAudited(Guid suspensionId, bool audited)
        {
            var result = await apiGateway.Post<SuspensionModel, UpdateSuspensionAuditStateModel>(
                new UpdateSuspensionAuditStateModel { Audited = audited },
                "suspensions",
                routeValues: new string[] { suspensionId.ToString(), "audit" }).ConfigureAwait(false);

            UpdateState(result);
        }

        public async Task AddTagToSuspension(Guid suspensionId, Guid tagId)
        {
            var result = await apiGateway.Post<SuspensionModel, AddTagToSuspensionModel>(
                new AddTagToSuspensionModel { TagId = tagId },
                "suspensions",
                routeValues: new string[] { suspensionId.ToString(), "tags" }).ConfigureAwait(false);

            UpdateState(result);
        }

        public async Task RemoveTagFromSuspension(Guid suspensionId, Guid tagId)
        {
            var result = await apiGateway.Delete<SuspensionModel, DeleteTagFromSuspensionModel>(
                new DeleteTagFromSuspensionModel { TagId = tagId },
                "suspensions",
                routeValues: new string[] { suspensionId.ToString(), "tags" }).ConfigureAwait(false);

            UpdateState(result);
        }

        public async Task CreateNewSuspension(string username)
        {
            var result = await apiGateway.Post<SuspensionModel, CreateSuspensionModel>(
                new CreateSuspensionModel { TwitchUsername = username, ChannelOfOrigin = CurrentlySelectedChannel },
                "suspensions").ConfigureAwait(false);

            UpdateState(result);
        }

        public async Task UploadImage(Guid suspensionId, MemoryStream memoryStream, string filename)
        {
            await apiGateway.PostImage(memoryStream, filename, "Suspensions", routeValues: new string[] { suspensionId.ToString(), "image" }).ConfigureAwait(false);
        }

        private void UpdateState(SuspensionModel model)
        {
            if (model == null)
                return;

            var existingValue = Suspensions.Find(x => x.SuspensionId == model.SuspensionId);
            Suspensions.Remove(existingValue);

            if (model.Timestamp.Date.ToLocalTime().Date == SelectedDate.Date)
                Suspensions.Add(model);

            NotifyStateChanged();
        }

        public void Dispose()
        {
            suspensionsHub.DisposeAsync();
        }
    }
}
