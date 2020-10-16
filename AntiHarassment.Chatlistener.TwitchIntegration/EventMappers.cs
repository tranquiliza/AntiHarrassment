using AntiHarassment.Chatlistener.Core.Events;
using TwitchLib.Client.Events;

namespace AntiHarassment.Chatlistener.TwitchIntegration
{
    internal static class EventMappers
    {
        internal static MessageReceivedEvent Map(this OnMessageReceivedArgs from, bool autoModded, bool deleted)
        {
            return new MessageReceivedEvent
            {
                TwitchMessageId = from.ChatMessage.Id,
                Message = from.ChatMessage.Message,
                Channel = from.ChatMessage.Channel,
                DisplayName = from.ChatMessage.DisplayName,
                AutoModded = autoModded,
                Deleted = deleted
            };
        }

        internal static ExistingUsersDetectedEvent Map(this OnExistingUsersDetectedArgs from)
        {
            return new ExistingUsersDetectedEvent
            {
                Users = from.Users,
                Channel = from.Channel
            };
        }

        internal static UserJoinedEvent Map(this OnUserJoinedArgs from)
        {
            return new UserJoinedEvent
            {
                Channel = from.Channel,
                Username = from.Username
            };
        }

        internal static UserLeftEvent Map(this OnUserLeftArgs from)
        {
            return new UserLeftEvent
            {
                Channel = from.Channel,
                Username = from.Username
            };
        }

        internal static UserBannedEvent Map(this OnUserBannedArgs from)
        {
            return new UserBannedEvent
            {
                Username = from.UserBan.Username,
                BanReason = from.UserBan.BanReason,
                Channel = from.UserBan.Channel
            };
        }

        internal static UserTimedoutEvent Map(this OnUserTimedoutArgs from)
        {
            return new UserTimedoutEvent
            {
                Channel = from.UserTimeout.Channel,
                TimeoutDuration = from.UserTimeout.TimeoutDuration,
                TimeoutReason = from.UserTimeout.TimeoutReason,
                Username = from.UserTimeout.Username
            };
        }

        internal static CommandReceivedEvent Map(this OnChatCommandReceivedArgs from)
        {
            return new CommandReceivedEvent
            {
                ArgumentsAsList = from.Command.ArgumentsAsList,
                ArgumentsAsString = from.Command.ArgumentsAsString,
                Channel = from.Command.ChatMessage.Channel,
                IsModerator = from.Command.ChatMessage.IsModerator,
                IsSubscriber = from.Command.ChatMessage.IsSubscriber,
                IsBroadcaster = from.Command.ChatMessage.IsBroadcaster,
                Username = from.Command.ChatMessage.DisplayName,
                UserType = from.Command.ChatMessage.UserType.Map(),
                CommandIdentifier = from.Command.CommandIdentifier,
                CommandText = from.Command.CommandText
            };
        }

        private static UserType Map(this TwitchLib.Client.Enums.UserType userType)
        {
            return userType switch
            {
                TwitchLib.Client.Enums.UserType.Viewer => UserType.Viewer,
                TwitchLib.Client.Enums.UserType.Moderator => UserType.Moderator,
                TwitchLib.Client.Enums.UserType.GlobalModerator => UserType.GlobalModerator,
                TwitchLib.Client.Enums.UserType.Broadcaster => UserType.Broadcaster,
                TwitchLib.Client.Enums.UserType.Admin => UserType.Admin,
                TwitchLib.Client.Enums.UserType.Staff => UserType.Staff,
                _ => UserType.Viewer,
            };
        }
    }
}
