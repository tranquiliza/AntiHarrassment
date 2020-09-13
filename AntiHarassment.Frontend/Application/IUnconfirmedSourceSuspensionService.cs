using AntiHarassment.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public interface IUnconfirmedSourceSuspensionService
    {
        string CurrentInvalidationReason { get; set; }
        SuspensionModel CurrentlySelectedSuspensionForInvalidation { get; set; }
        Task UpdateSuspensionValidity(Guid suspensionId, bool invalidate, string invalidationReason = "");

        string UserLinkReason { get; set; }
        string CurrentSearchTerm { get; set; }
        SuspensionModel CurrentlySelectedSuspension { get; set; }
        Task AddUserLinkToSuspension(Guid suspensionId, string twitchUsername, string linkReason);
        Task RemoveUserLinkFromSuspension(Guid suspensionId, string twitchUsername);

        Task UpdateAudited(Guid suspensionId, bool audited);
        Task AddTagToSuspension(Guid suspensionId, Guid tagId);
        Task RemoveTagFromSuspension(Guid suspensionId, Guid tagId);

        event Action OnChange;
        List<SuspensionModel> Suspensions { get; }
        Task Initialize();
        Task FetchSuspensions();
    }
}