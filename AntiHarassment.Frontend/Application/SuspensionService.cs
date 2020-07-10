using AntiHarassment.Contract;
using AntiHarassment.Frontend.Infrastructure;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public class SuspensionService : ISuspensionService
    {
        public string CurrentlySelectedChannel { get; private set; }
        public List<SuspensionModel> Suspensions { get; private set; }

        private readonly IApiGateway apiGateway;

        public SuspensionService(IApiGateway apiGateway)
        {
            this.apiGateway = apiGateway;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public event Action OnChange;

        public async Task FetchSuspensionForChannel(string channelName)
        {
            Suspensions = await apiGateway.Get<List<SuspensionModel>>("suspensions", routeValues: new string[] { channelName }).ConfigureAwait(false);
            CurrentlySelectedChannel = channelName;
            NotifyStateChanged();
        }
    }
}
