using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface INotificationService
    {
        void AddUser(string connectionId, string twitchUsername);
        void RemoveUser(string connectionId);
        Task<List<string>> GetRecipientsFor(string channelOfOrigin);
    }
}
