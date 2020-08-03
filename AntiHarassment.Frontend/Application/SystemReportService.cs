using AntiHarassment.Contract;
using AntiHarassment.Frontend.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public class SystemReportService : ISystemReportService
    {
        public SystemReportModel SystemReportModel { get; private set; }

        private readonly IApiGateway apiGateway;

        private void NotifyStateChanged() => OnChange?.Invoke();
        public event Action OnChange;

        public SystemReportService(IApiGateway apiGateway)
        {
            this.apiGateway = apiGateway;
        }

        public async Task Initialize()
        {
            SystemReportModel = await apiGateway.Get<SystemReportModel>("system", routeValues: new string[] { "report" }).ConfigureAwait(false);
            NotifyStateChanged();
        }
    }
}
