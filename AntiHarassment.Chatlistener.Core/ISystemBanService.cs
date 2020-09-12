using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface ISystemBanService
    {
        Task IssueBanFor(string username, string channelToBanFrom, string systemReason);
    }
}
