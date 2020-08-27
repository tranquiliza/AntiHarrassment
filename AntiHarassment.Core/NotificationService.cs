using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public class NotificationService : INotificationService
    {
        private class ConnectionRelation
        {
            public string TwitchUsername { get; private set; }
            public string ConnectionId { get; private set; }

            public ConnectionRelation(string twitchUsername, string connectionId)
            {
                TwitchUsername = twitchUsername;
                ConnectionId = connectionId;
            }
        }

        private static readonly List<ConnectionRelation> userLookup = new List<ConnectionRelation>();
        private readonly IChannelRepository channelRepository;

        public NotificationService(IChannelRepository channelRepository)
        {
            this.channelRepository = channelRepository;
        }

        public void AddUser(string connectionId, string twitchUsername)
        {
            if (!userLookup.Any(x => x.ConnectionId == connectionId))
                userLookup.Add(new ConnectionRelation(twitchUsername, connectionId));
        }

        public void RemoveUser(string connectionId)
        {
            var relation = userLookup.Find(x => x.ConnectionId == connectionId);
            userLookup.Remove(relation);
        }

        public async Task<List<string>> GetRecipientsFor(string channelOfOrigin)
        {
            var channel = await channelRepository.GetChannel(channelOfOrigin).ConfigureAwait(false);
            var mods = channel.Moderators;
            var modsOnDuty = userLookup.Where(x => mods.Any(y => string.Equals(y, x.TwitchUsername, StringComparison.OrdinalIgnoreCase))
            || string.Equals(x.TwitchUsername, channelOfOrigin, StringComparison.OrdinalIgnoreCase)).Distinct();
            return modsOnDuty.Select(x => x.ConnectionId).ToList();
        }
    }
}
