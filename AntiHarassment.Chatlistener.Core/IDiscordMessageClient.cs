using System;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface IDiscordMessageClient : IAsyncDisposable
    {
        Task Initialize();
        Task SendMessageToPrometheus(string message);
        Task SendDmToUser(ulong discordUserId, string message);
    }
}
