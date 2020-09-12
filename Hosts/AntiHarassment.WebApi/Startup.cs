using AntiHarassment.WebApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AntiHarassment.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddDefaultPolicy(builder => builder.WithOrigins("https://antiharassment.azurewebsites.net", "https://localhost:44394").AllowAnyHeader().AllowAnyMethod()));

            services.AddControllers();
            services.AddSignalR();

            var applicationConfiguration = ApplicationConfiguration.Readfrom(Configuration);

            services.RegisterApplicationServices(Configuration, applicationConfiguration);

            var key = Encoding.ASCII.GetBytes(applicationConfiguration.SecurityKey);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddMvc(options => options.Filters.Add(typeof(ApplicationContextFilter)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<Hubs.ChannelsHub>(SignalR.Contract.ChannelsHubSignalRClient.HUBURL);
                endpoints.MapHub<Hubs.SuspensionsHub>(SignalR.Contract.SuspensionsHubSignalRClient.HUBURL);
                endpoints.MapHub<Hubs.NotificationHub>(SignalR.Contract.NotificationHubSignalRClient.HUBURL);
            });
        }
    }
}
