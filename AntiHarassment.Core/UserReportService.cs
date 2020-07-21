using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public class UserReportService : IUserReportService
    {
        private readonly ISuspensionRepository suspensionRepository;

        public UserReportService(ISuspensionRepository suspensionRepository)
        {
            this.suspensionRepository = suspensionRepository;
        }

        public async Task<IResult<UserReport>> GetUserReportFor(string username)
        {
            var suspensionsForUser = await suspensionRepository.GetSuspensionsForUser(username).ConfigureAwait(false);
            if (suspensionsForUser.Count == 0)
                return Result<UserReport>.NoContentFound();

            var userReport = new UserReport(username, suspensionsForUser);
            if (userReport.Suspensions.Count == 0)
                return Result<UserReport>.NoContentFound();

            return Result<UserReport>.Succeeded(userReport);
        }
    }
}
