using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Chatlistener.TwitchIntegration
{
    public class TwitchClientSettings
    {
        public string TwitchUsername { get; set; }
        public string TwitchBotOAuth { get; set; }

        public TwitchClientSettings(string twitchUsername, string twitchBotOAuth)
        {
            TwitchUsername = twitchUsername;
            TwitchBotOAuth = twitchBotOAuth;
        }
    }
}
