using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Chatlistener
{
    public static class DependencyInjection
    {
        public static IHostBuilder RegisterApplicationServices(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((context, services) =>
            {

            });
        }
    }
}
