using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core.Repositories
{
    public interface IChatterRepository
    {
        Task UpsertChatter(string twitchUsername, DateTime timestamp);
    }
}
