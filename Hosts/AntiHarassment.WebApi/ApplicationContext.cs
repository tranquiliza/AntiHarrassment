using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi
{
    public class ApplicationContext : IApplicationContext
    {
        public User User { get; private set; }

        public Guid UserId => User?.Id ?? default;

        public static ApplicationContext Create(User user) => new ApplicationContext { User = user };
    }
}
