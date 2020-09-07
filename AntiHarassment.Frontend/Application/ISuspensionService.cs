using AntiHarassment.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public interface ISuspensionService
    {
        string CurrentlySelectedChannel { get; }
        List<string> UsersFromChannel { get; }

        List<SuspensionModel> Suspensions { get; }
        List<ChannelModel> Channels { get; }

        public SuspensionModel CurrentlySelectedSuspensionForImages { get; }

        SuspensionModel CurrentlySelectedSuspensionForInvalidation { get; set; }
        string CurrentInvalidationReason { get; set; }

        string UserLinkReason { get; set; }
        string CurrentSearchTerm { get; set; }
        SuspensionModel CurrentlySelectedSuspension { get; set; }
        List<DateTime> DatesWithUnauditedSuspensions { get; set; }

        event Action OnChange;

        Task FetchSuspensionForChannel(string channelName, DateTime date);
        Task FetchSeenUsersForChannel(string channelName);
        Task UpdateSuspensionValidity(Guid suspensionId, bool invalidate, string invalidationReason = "");
        Task UpdateAudited(Guid suspensionId, bool audited);
        Task AddTagToSuspension(Guid suspensionId, Guid tagId);
        Task RemoveTagFromSuspension(Guid suspensionId, Guid tagId);
        Task AddUserLinkToSuspension(Guid suspensionId, string twitchUsername, string linkReason);
        Task Initialize();
        Task RemoveUserLinkFromSuspension(Guid suspensionId, string twitchUsername);
        Task CreateNewSuspension(string username);
        Task UploadImage(Guid suspensionId, MemoryStream memoryStream, string filename);
        void SetCurrentlySelectedSuspensionForImages(SuspensionModel suspension);
        Task FetchDaysWithUnauditedSuspensions(string channelName);
    }
}