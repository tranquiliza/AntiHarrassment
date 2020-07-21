using AntiHarassment.Contract;
using AntiHarassment.Frontend.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public class UserReportService : IUserReportService
    {
        public string UserReportLookupError { get; private set; }
        public UserReportModel UserReport { get; private set; }

        private readonly IApiGateway apiGateway;

        public UserReportService(IApiGateway apiGateway)
        {
            this.apiGateway = apiGateway;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public event Action OnChange;

        public async Task SearchForUser(string username)
        {
            UserReport = null;
            UserReportLookupError = "";

            var qParam = new QueryParam("username", username);

            try
            {
                UserReport = await apiGateway.Get<UserReportModel>("UserReports", queryParams: qParam).ConfigureAwait(false);
                if (UserReport == null)
                {
                    UserReportLookupError = "No user found with this name";
                }
            }
            catch (Exception ex)
            {
                UserReportLookupError = ex.Message;
            }

            NotifyStateChanged();
        }
    }
}
