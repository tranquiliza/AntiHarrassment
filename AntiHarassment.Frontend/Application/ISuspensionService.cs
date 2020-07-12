using AntiHarassment.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public interface ISuspensionService
    {
        List<SuspensionModel> Suspensions { get; }
        string CurrentlySelectedChannel { get; }

        event Action OnChange;

        Task FetchSuspensionForChannel(string channelName);
    }
}