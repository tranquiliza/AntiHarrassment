using AntiHarassment.Contract;
using AntiHarassment.Frontend.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public class UserChannelService : IUserChannelService
    {
        private readonly IApiGateway apiGateway;
        private readonly IUserService userService;

        public ChannelModel Channel { get; private set; }

        public UserChannelService(IApiGateway apiGateway, IUserService userService)
        {
            this.apiGateway = apiGateway;
            this.userService = userService;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public event Action OnChange;

        public async Task Initialize()
        {
            if (userService.IsUserLoggedIn)
            {
                var channelParameter = new QueryParam("channelName", userService.CurrentUserTwitchUsername);
                Channel = await apiGateway.Get<ChannelModel>("channels", queryParams: new QueryParam[] { channelParameter }).ConfigureAwait(false);

                NotifyStateChanged();
            }
        }

        public async Task AddModerator(string moderatorTwitchUsername)
        {
            var model = new AddModeratorModel { ModeratorTwitchUsername = moderatorTwitchUsername };
            Channel = await apiGateway.Post<ChannelModel, AddModeratorModel>(model, "channels", routeValues: new string[] { userService.CurrentUserTwitchUsername }).ConfigureAwait(false);

            NotifyStateChanged();
        }

        public async Task RemoveModerator(string moderatorTwitchUsername)
        {
            var model = new DeleteModeratorModel { ModeratorTwitchUsername = moderatorTwitchUsername };
            Channel = await apiGateway.Delete<ChannelModel, DeleteModeratorModel>(model, "channels", routeValues: new string[] { userService.CurrentUserTwitchUsername }).ConfigureAwait(false);

            NotifyStateChanged();
        }
    }
}
