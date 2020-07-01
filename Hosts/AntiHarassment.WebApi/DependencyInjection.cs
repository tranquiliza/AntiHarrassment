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
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connstring = configuration["ConnectionStrings:AntiHarassmentDatabase"];

            services.AddSingleton<IChannelService, ChannelService>();
            services.AddSingleton<IChannelRepository, ChannelRepository>(_ => new ChannelRepository(connstring));
            services.AddSingleton<IMessageDispatcher, MessageDispatcher>();

            return services;
        }
    }
}
