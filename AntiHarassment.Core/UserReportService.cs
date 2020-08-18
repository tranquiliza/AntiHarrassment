using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IResult<List<string>>> GetUsersMatchedByTag(Guid tagId)
        {
            var suspensionsForSystem = await suspensionRepository.GetSuspensions(DateTime.UtcNow.AddYears(-10)).ConfigureAwait(false);
            if (suspensionsForSystem.Count == 0)
                return Result<List<string>>.NoContentFound();

            var suspensionsWithTag = suspensionsForSystem.Where(x =>
                    x.Audited
                && !x.InvalidSuspension
                && x.Tags.Any(y => y.TagId == tagId)
            );

            var users = suspensionsWithTag.Select(x => x.Username).OrderBy(x => x);
            return Result<List<string>>.Succeeded(users.ToList());
        }
    }
}
