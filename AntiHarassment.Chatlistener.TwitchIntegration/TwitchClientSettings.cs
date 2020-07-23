using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Chatlistener.TwitchIntegration
{
    public class TwitchClientSettings
    {
        public string TwitchUsername { get; set; }
        public string TwitchBotOAuth { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }

        public TwitchClientSettings(string twitchUsername, string twitchBotOAuth, string clientId, string secret)
        {
            TwitchUsername = twitchUsername;
            TwitchBotOAuth = twitchBotOAuth;
            ClientId = clientId;
            Secret = secret;
        }
    }
}
