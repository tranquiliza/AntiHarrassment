using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;

namespace AntiHarassment.Chatlistener
{
    public class ApplicatonContext : IApplicationContext
    {
        public User User { get; private set; }

        public Guid UserId => User?.Id ?? default;

        public static ApplicatonContext CreateFromUser(User user) => new ApplicatonContext { User = user };
    }
}
