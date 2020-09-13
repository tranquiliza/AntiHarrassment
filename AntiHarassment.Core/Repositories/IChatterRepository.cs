using System;
using System.Threading.Tasks;

namespace AntiHarassment.Core.Repositories
{
    public interface IChatterRepository
    {
        Task UpsertChatter(string twitchUsername, DateTime timestamp);
    }
}
