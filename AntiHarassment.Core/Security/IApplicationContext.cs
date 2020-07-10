using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Core.Security
{
    public interface IApplicationContext
    {
        User User { get; }
        Guid UserId { get; }
    }
}
