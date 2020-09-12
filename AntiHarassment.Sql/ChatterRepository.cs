using AntiHarassment.Core.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AntiHarassment.Sql
{
    public class ChatterRepository : IChatterRepository
    {
        private readonly ISqlAccess sql;
        private readonly ILogger<ChatterRepository> logger;

        public ChatterRepository(string connectionString, ILogger<ChatterRepository> logger)
        {
            this.sql = SqlAccessBase.Create(connectionString);
            this.logger = logger;
        }

        public async Task UpsertChatter(string twitchUsername, DateTime timestamp)
        {
            try
            {
                using var command = sql.CreateStoredProcedure("[Core].[UpsertChatter]");
                command.WithParameter("@twitchUsername", twitchUsername)
                    .WithParameter("@firstTimeSeen", timestamp);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when attempting to save new chatter");
                throw;
            }
        }
    }
}
