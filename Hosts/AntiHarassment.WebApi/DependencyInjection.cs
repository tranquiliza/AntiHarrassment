using AntiHarassment.Core;
using AntiHarassment.Messaging.NServiceBus;
using AntiHarassment.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration, ApplicationConfiguration applicationConfiguration)
        {
            var connstring = configuration["ConnectionStrings:AntiHarassmentDatabase"];

            services.AddSingleton<IChannelService, ChannelService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ISuspensionService, SuspensionService>();

            services.AddSingleton<ISecurity, PasswordSecurity>();
            services.AddSingleton<IApplicationConfiguration>(applicationConfiguration);

            services.AddSingleton<IChannelRepository, ChannelRepository>(_ => new ChannelRepository(connstring));
            services.AddSingleton<IUserRepository, UserRepository>(_ => new UserRepository(connstring));
            services.AddSingleton<ISuspensionRepository, SuspensionRepository>(_ => new SuspensionRepository(connstring));

            services.AddSingleton<IMessageDispatcher, MessageDispatcher>();

            return services;
        }
    }
}
