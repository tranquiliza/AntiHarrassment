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
    public class UnconfirmedSourceSuspensionService : IUnconfirmedSourceSuspensionService, IDisposable
    {
        public List<SuspensionModel> Suspensions { get; private set; }
        public string CurrentInvalidationReason { get; set; }
        public SuspensionModel CurrentlySelectedSuspensionForInvalidation { get; set; }
        public string UserLinkReason { get; set; }
        public string CurrentSearchTerm { get; set; }
        public SuspensionModel CurrentlySelectedSuspension { get; set; }

        private readonly IApiGateway apiGateway;
        private readonly SuspensionsHubSignalRClient suspensionsHub;

        public UnconfirmedSourceSuspensionService(IApiGateway apiGateway, SuspensionsHubSignalRClient suspensionsHub)
        {
            this.apiGateway = apiGateway;
            this.suspensionsHub = suspensionsHub;

            suspensionsHub.OnNewSuspension += async (sender, args) => await SuspensionsHub_OnNewSuspension(sender, args);
            suspensionsHub.OnSuspensionUpdated += async (sender, args) => await SuspensionsHub_SuspensionUpdated(sender, args);
        }

        public event Action OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        private async Task SuspensionsHub_SuspensionUpdated(object _, SuspensionUpdatedEventArgs args)
        {
            if (Suspensions.Any(x => x.SuspensionId == args.SuspensionId))
            {
                var qParam = new QueryParam("suspensionId", args.SuspensionId.ToString());
                var newSuspension = await apiGateway.Get<SuspensionModel>("suspensions", queryParams: qParam).ConfigureAwait(false);

                UpdateState(newSuspension);
            }
        }

        private async Task SuspensionsHub_OnNewSuspension(object _, NewSuspensionEventArgs args)
        {
            var qParam = new QueryParam("suspensionId", args.SuspensionId.ToString());
            var newSuspension = await apiGateway.Get<SuspensionModel>("suspensions", queryParams: qParam).ConfigureAwait(false);
            UpdateState(newSuspension);
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

        public async Task Initialize()
        {
            var result = await apiGateway.Get<List<SuspensionModel>>("suspensions", routeValues: new string[] { "unconfirmed" }).ConfigureAwait(false);
            Suspensions = result ?? new List<SuspensionModel>();

            await suspensionsHub.StartAsync().ConfigureAwait(false);

            NotifyStateChanged();
        }

        public async Task UpdateSuspensionValidity(Guid suspensionId, bool invalidate, string invalidationReason = "")
        {
            var result = await apiGateway.Post<SuspensionModel, MarkSuspensionValidityModel>(
                new MarkSuspensionValidityModel { Invalidate = invalidate, InvalidationReason = invalidationReason },
                "suspensions",
                routeValues: new string[] { suspensionId.ToString(), "validity" }).ConfigureAwait(false);

            CurrentInvalidationReason = "";

            UpdateState(result);
        }

        public async Task AddUserLinkToSuspension(Guid suspensionId, string twitchUsername, string linkReason)
        {
            var result = await apiGateway.Post<SuspensionModel, AddUserLinkToSuspensionModel>(
                new AddUserLinkToSuspensionModel { Username = twitchUsername, LinkUserReason = linkReason },
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

        public void Dispose()
        {
            suspensionsHub.DisposeAsync();
        }
    }
}
