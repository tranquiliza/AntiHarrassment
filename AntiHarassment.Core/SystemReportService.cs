using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public class SystemReportService : ISystemReportService
    {
        private readonly ISuspensionRepository suspensionRepository;
        private readonly IChatRepository chatRepository;
        private readonly IDatetimeProvider datetimeProvider;

        public SystemReportService(ISuspensionRepository suspensionRepository, IChatRepository chatRepository, IDatetimeProvider datetimeProvider)
        {
            this.suspensionRepository = suspensionRepository;
            this.chatRepository = chatRepository;
            this.datetimeProvider = datetimeProvider;
        }

        public async Task<IResult<SystemReport>> GetSystemReport(IApplicationContext context)
        {
            if (!context.User.HasRole(Roles.Admin))
                return Result<SystemReport>.Unauthorized();

            var allSuspensions = await suspensionRepository.GetSuspensions(datetimeProvider.UtcNow.AddDays(-30)).ConfigureAwait(false);
            var systemSuspensions = allSuspensions.Where(x => x.SuspensionSource == SuspensionSource.System).ToList();
            var allSuspensionsWithoutSystem = allSuspensions.Where(x => x.SuspensionSource != SuspensionSource.System);

            var auditedSuspensions = allSuspensionsWithoutSystem.Where(x => x.Audited).ToList();
            var unauditedSuspensins = allSuspensionsWithoutSystem.Where(x => !x.Audited).ToList();
            var uniqueUsersForSystem = await chatRepository.GetUniqueChattersForSystem().ConfigureAwait(false);

            var systemReport = new SystemReport(unauditedSuspensins, auditedSuspensions, systemSuspensions, uniqueUsersForSystem.Count);

            return Result<SystemReport>.Succeeded(systemReport);
        }
    }
}
