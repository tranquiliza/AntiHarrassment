using AntiHarassment.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public interface ISuspensionService
    {
        string CurrentlySelectedChannel { get; }
        List<string> UsersFromChannel { get; }

        List<SuspensionModel> Suspensions { get; }
        List<ChannelModel> Channels { get; }

        SuspensionModel CurrentlySelectedSuspensionForInvalidation { get; set; }
        string CurrentInvalidationReason { get; set; }

        string CurrentSearchTerm { get; set; }
        SuspensionModel CurrentlySelectedSuspension { get; set; }


        event Action OnChange;

        Task FetchSuspensionForChannel(string channelName);
        Task FetchSeenUsersForChannel(string channelName);
        Task UpdateSuspensionValidity(Guid suspensionId, bool invalidate, string invalidationReason = "");
        Task UpdateAudited(Guid suspensionId, bool audited);
        Task AddTagToSuspension(Guid suspensionId, Guid tagId);
        Task RemoveTagFromSuspension(Guid suspensionId, Guid tagId);
        Task AddUserLinkToSuspension(Guid suspensionId, string twitchUsername);
        Task Initialize();
        Task RemoveUserLinkFromSuspension(Guid suspensionId, string twitchUsername);
        Task CreateNewSuspension(string username);
    }
}