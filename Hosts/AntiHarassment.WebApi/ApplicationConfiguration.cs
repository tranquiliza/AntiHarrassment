using AntiHarassment.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public string SecurityKey { get; private set; }

        public static ApplicationConfiguration Readfrom(IConfiguration configuration)
        {
            return new ApplicationConfiguration
            {
                SecurityKey = configuration["ApplicationSettings:TokenSecret"]
            };
        }
    }
}
