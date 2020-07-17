using AntiHarassment.Contract;
using AntiHarassment.Contract.Suspensions;
using AntiHarassment.Frontend.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public class SuspensionService : ISuspensionService
    {
        public List<ChannelModel> Channels { get; private set; }

        public string CurrentlySelectedChannel { get; private set; }
        public List<SuspensionModel> Suspensions { get; private set; }

        private readonly IApiGateway apiGateway;
        private readonly IUserService userService;

        public SuspensionService(IApiGateway apiGateway, IUserService userService)
        {
            this.apiGateway = apiGateway;
            this.userService = userService;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public event Action OnChange;

        public async Task Initialize()
        {
            Channels = await apiGateway.Get<List<ChannelModel>>("Channels").ConfigureAwait(false);

            if (!Channels.Any(x => string.Equals(x.ChannelName, userService.CurrentUserTwitchUsername, StringComparison.OrdinalIgnoreCase)))
            {
                if (Channels.Count > 0)
                    await FetchSuspensionForChannel(Channels[0].ChannelName).ConfigureAwait(false);
            }
            else
            {
                await FetchSuspensionForChannel(userService.CurrentUserTwitchUsername).ConfigureAwait(false);
            }

            NotifyStateChanged();
        }

        public async Task FetchSuspensionForChannel(string channelName)
        {
            var result = await apiGateway.Get<List<SuspensionModel>>("suspensions", routeValues: new string[] { channelName }).ConfigureAwait(false);
            Suspensions = result ?? new List<SuspensionModel>();

            CurrentlySelectedChannel = channelName;
            NotifyStateChanged();
        }

        public async Task UpdateSuspensionValidity(Guid suspensionId, bool invalidate)
        {
            var result = await apiGateway.Post<SuspensionModel, MarkSuspensionValidityModel>(
                new MarkSuspensionValidityModel { Invalidate = invalidate },
                "suspensions",
                routeValues: new string[] { suspensionId.ToString(), "validity" }).ConfigureAwait(false);

            UpdateState(result);
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

        private void UpdateState(SuspensionModel model)
        {
            if (model == null)
                return;

            var existingValue = Suspensions.Find(x => x.SuspensionId == model.SuspensionId);
            Suspensions.Remove(existingValue);
            Suspensions.Add(model);
            NotifyStateChanged();
        }
    }
}
