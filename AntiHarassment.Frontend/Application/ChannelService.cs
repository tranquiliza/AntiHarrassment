using AntiHarassment.Contract;
using AntiHarassment.Frontend.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public class ChannelService : IChannelService
    {
        public List<ChannelModel> Channels { get; private set; }

        private readonly IApiGateway apiGateway;
        private readonly IUserService userService;

        public ChannelService(IApiGateway apiGateway, IUserService userService)
        {
            this.apiGateway = apiGateway;
            this.userService = userService;

            userService.OnChange += UserService_OnChange;
        }

        private void UserService_OnChange()
        {
            if (!userService.IsUserLoggedIn)
                Channels = null;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public event Action OnChange;

        public async Task Initialize()
        {
            if (userService.IsUserLoggedIn)
            {
                Channels = await apiGateway.Get<List<ChannelModel>>("Channels").ConfigureAwait(false);
                NotifyStateChanged();
            }
        }
    }
}
