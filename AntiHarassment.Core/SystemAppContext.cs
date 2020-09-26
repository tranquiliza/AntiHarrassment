using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;

namespace AntiHarassment.Core
{
    public class SystemAppContext : IApplicationContext
    {
        public SystemAppContext()
        {
            User = User.CreateSystemUser();
        }

        public User User { get; }

        public Guid UserId => User.Id;
    }
}
