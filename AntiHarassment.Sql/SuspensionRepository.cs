using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Sql
{
    public class SuspensionRepository : ISuspensionRepository
    {
        private readonly ISqlAccess sql;
        public SuspensionRepository(string connectionString)
        {
            sql = SqlAccessBase.Create(connectionString);
        }

        public async Task<List<Suspension>> GetSuspensionsForChannel(string channelOfOrigin)
        {
            var result = new List<Suspension>();
            
            using (var command = sql.CreateStoredProcedure("[Core].[GetSuspensionsForChannel]"))
            {
                command.WithParameter("channelOfOrigin", channelOfOrigin);
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        result.Add(Serialization.Deserialize<Suspension>(reader.GetString("data")));
                    }
                }
            }

            return result;
        }

        public async Task SaveSuspension(Suspension suspension)
        {
            using (var command = sql.CreateStoredProcedure("[Core].[InsertSuspension]"))
            {
                command
                    .WithParameter("suspensionId", suspension.SuspensionId)
                    .WithParameter("username", suspension.Username)
                    .WithParameter("channelOfOrigin", suspension.ChannelOfOrigin)
                    .WithParameter("typeOfSuspension", suspension.SuspensionType.ToString())
                    .WithParameter("timestamp", suspension.Timestamp)
                    .WithParameter("duration", suspension.Duration)
                    .WithParameter("data", Serialization.Serialize(suspension));

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}
