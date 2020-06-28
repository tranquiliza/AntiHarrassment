using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Chatlistener.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Sql
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
            try
            {
                var result = new List<Channel>();

                using (var command = sql.CreateStoredProcedure("[Core].[GetChannels]"))
                using (var reader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.Default).ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                        result.Add(new Channel(reader.GetString("channelName"), reader.GetBoolean("shouldListen")));

                    return result;
                }
            }
            catch (Exception ex)
            {
                // LOG
                throw;
            }
        }
    }
}
