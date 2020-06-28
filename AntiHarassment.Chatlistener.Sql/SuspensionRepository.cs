using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Chatlistener.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Sql
{
    public class SuspensionRepository : ISuspensionRepository
    {
        private readonly ISqlAccess sql;
        public SuspensionRepository(string connectionString)
        {
            sql = SqlAccessBase.Create(connectionString);
        }

        public async Task SaveSuspension(Suspension suspension)
        {
            try
            {
                using (var command = sql.CreateStoredProcedure("[Core].[InsertSuspension]"))
                {
                    command.WithParameter("username", suspension.Username)
                        .WithParameter("channelOfOrigin", suspension.ChannelOfOrigin)
                        .WithParameter("typeOfSuspension", suspension.SuspensionType.ToString())
                        .WithParameter("timestamp", suspension.Timestamp)
                        .WithParameter("duration", suspension.Duration);

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
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
