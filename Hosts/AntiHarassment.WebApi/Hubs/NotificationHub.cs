using AntiHarassment.Core;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly INotificationService notificationService;

        public NotificationHub(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public Task Register(string twitchUsername)
        {
            var currentId = Context.ConnectionId;

            notificationService.AddUser(currentId, twitchUsername);

            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception e)
        {
            notificationService.RemoveUser(Context.ConnectionId);

            return base.OnDisconnectedAsync(e);
        }
    }
}
