using AntiHarassment.Core.Models;
using System;

namespace AntiHarassment.Core.Security
{
    public interface IApplicationContext
    {
        User User { get; }
        Guid UserId { get; }
    }
}
