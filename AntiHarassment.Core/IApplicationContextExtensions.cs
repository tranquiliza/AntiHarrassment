using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Core
{
    public static class IApplicationContextExtensions
    {
        public static bool HaveAccessTo(this IApplicationContext context, Channel channel)
        {
            return string.Equals(context.User?.TwitchUsername, channel.ChannelName, StringComparison.OrdinalIgnoreCase)
                            || context.User?.HasRole(Roles.Admin) == true
                            || channel.HasModerator(context.User?.TwitchUsername);
        }

        public static bool HaveOwnerAccessTo(this IApplicationContext context, Channel channel)
        {
            return string.Equals(context.User?.TwitchUsername, channel.ChannelName, StringComparison.OrdinalIgnoreCase)
                || context.User?.HasRole(Roles.Admin) == true;
        }
    }
}
