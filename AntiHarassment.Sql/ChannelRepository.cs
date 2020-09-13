using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntiHarassment.Sql
{
    public class ChannelRepository : IChannelRepository
    {
        private readonly ISqlAccess sql;
        private readonly ILogger<ChannelRepository> logger;

        public ChannelRepository(string connectionString, ILogger<ChannelRepository> logger)
        {
            sql = SqlAccessBase.Create(connectionString);
            this.logger = logger;
        }

        public async Task<List<Channel>> GetChannels()
        {
            try
            {
                var result = new List<Channel>();
                using var command = sql.CreateStoredProcedure("[Core].[GetChannels]");
                using var reader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.Default).ConfigureAwait(false);
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var channel = Serialization.Deserialize<Channel>(reader.GetString("data"));
                    result.Add(channel);
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error getting channels");
                throw;
            }
        }

        public async Task<Channel> GetChannel(string twitchUsername)
        {
            try
            {
                using (var command = sql.CreateStoredProcedure("[Core].[GetChannel]"))
                {
                    command.WithParameter("twitchUsername", twitchUsername);
                    using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        return Serialization.Deserialize<Channel>(reader.GetString("data"));
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error getting specific channel {arg}", twitchUsername);
                throw;
            }
        }

        public async Task Upsert(Channel channel)
        {
            try
            {
                using var command = sql.CreateStoredProcedure("[Core].[UpsertChannel]");
                command.WithParameter("channelId", channel.ChannelId)
                    .WithParameter("channelName", channel.ChannelName)
                    .WithParameter("shouldListen", channel.ShouldListen)
                    .WithParameter("data", Serialization.Serialize(channel));

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when trying to save channel {arg}", channel.ChannelName);
                throw;
            }
        }
    }
}
