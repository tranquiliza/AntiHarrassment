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

        public SystemReportService(ISuspensionRepository suspensionRepository, IChatRepository chatRepository)
        {
            this.suspensionRepository = suspensionRepository;
            this.chatRepository = chatRepository;
        }

        public async Task<IResult<SystemReport>> GetSystemReport(IApplicationContext context)
        {
            //if (!context.User.HasRole(Roles.Admin))
            //    return Result<SystemReport>.Unauthorized();

            var allSuspensions = await suspensionRepository.GetSuspensions().ConfigureAwait(false);

            var auditedSuspensions = allSuspensions.Where(x => x.Audited).ToList();
            var unauditedSuspensins = allSuspensions.Where(x => !x.Audited).ToList();
            var uniqueUsersForSystem = await chatRepository.GetUniqueChattersForSystem().ConfigureAwait(false);

            var systemReport = new SystemReport(unauditedSuspensins, auditedSuspensions, uniqueUsersForSystem.Count);

            return Result<SystemReport>.Succeeded(systemReport);
        }
    }
}
