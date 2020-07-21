using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Sql
{
    public class ChannelRepository : IChannelRepository
    {
        private readonly ISqlAccess sql;

        public ChannelRepository(string connectionString)
        {
            sql = SqlAccessBase.Create(connectionString);
        }

        public async Task<List<Channel>> GetChannels()
        {
            var result = new List<Channel>();
            using (var command = sql.CreateStoredProcedure("[Core].[GetChannels]"))
            using (var reader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.Default).ConfigureAwait(false))
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var channel = Serialization.Deserialize<Channel>(reader.GetString("data"));
                    result.Add(channel);
                }

                return result;
            }
        }

        public async Task<Channel> GetChannel(string twitchUsername)
        {
            using (var command = sql.CreateStoredProcedure("[Core].[GetChannel]"))
            {
                command.WithParameter("twitchUsername", twitchUsername);
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        return Serialization.Deserialize<Channel>(reader.GetString("data"));
                    }
                }
            }

            return null;
        }

        public async Task Upsert(Channel channel)
        {
            using (var command = sql.CreateStoredProcedure("[Core].[UpsertChannel]"))
            {
                command.WithParameter("channelId", channel.ChannelId)
                    .WithParameter("channelName", channel.ChannelName)
                    .WithParameter("shouldListen", channel.ShouldListen)
                    .WithParameter("data", Serialization.Serialize(channel));

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}
