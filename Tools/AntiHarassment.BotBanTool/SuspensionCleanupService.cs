using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Tool
{
    public class SuspensionCleanupService
    {
        private readonly ISuspensionRepository suspensionRepository;

        public SuspensionCleanupService(ISuspensionRepository suspensionRepository)
        {
            this.suspensionRepository = suspensionRepository;
        }

        public async Task CleanupOops()
        {
            var systemContext = new SystemAppContext();
            var utcNow = DateTime.UtcNow;

            var suspensions = await suspensionRepository.GetSuspensions(DateTime.UtcNow.AddYears(-1)).ConfigureAwait(false);

            var timeOfOops = DateTime.Parse("2020-10-04T01:18:14.7468436Z");
            var timeOfFix = new DateTime(2020, 10, 7, 22, 0, 0);

            var suspensionsFromTime = suspensions.Where(x => x.Timestamp >= timeOfOops && x.Timestamp <= timeOfFix).ToList();

            var systemSuspensionsThatNeedReplay = suspensionsFromTime.Where(x => x.SuspensionSource == SuspensionSource.System);
            foreach (var suspension in systemSuspensionsThatNeedReplay)
            {
                suspension.UpdateValidity(true, "Tranquiliza made a mistake, rolling back system bans in the period to replay", systemContext, DateTime.UtcNow);

                await suspensionRepository.Save(suspension).ConfigureAwait(false);
            }

            var names = systemSuspensionsThatNeedReplay.Select(x => x.Username).Distinct().ToList();

            var actualSuspensions = suspensionsFromTime.Where(x => names.Contains(x.Username) && x.SuspensionSource != SuspensionSource.System);

            foreach (var suspension in actualSuspensions)
            {
                suspension.UpdateAuditedState(false, systemContext, utcNow);

                await suspensionRepository.Save(suspension).ConfigureAwait(false);
            }
        }
    }
}
