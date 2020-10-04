using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.DiscordIntegration
{
    public interface IDiscordClientSettings
    {
        string Token { get; }
        ulong ServerId { get; }
        ulong ChannelId { get; }
    }
}
