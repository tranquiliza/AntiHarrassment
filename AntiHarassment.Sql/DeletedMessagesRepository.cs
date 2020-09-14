using AntiHarassment.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Sql
{
    public class DeletedMessagesRepository : IDeletedMessagesRepository
    {
        private readonly ISqlAccess sql;
        private readonly ILogger<DeletedMessagesRepository> logger;

        public DeletedMessagesRepository(string connectionString, ILogger<DeletedMessagesRepository> logger)
        {
            this.sql = SqlAccessBase.Create(connectionString);
            this.logger = logger;
        }

        public async Task Insert(string channel, string username, string deletedBy, string message, DateTime timestamp)
        {
            try
            {
                using var command = sql.CreateStoredProcedure("[Core].[InsertDeletedMessage]");
                command.WithParameter("username", username)
                    .WithParameter("channelOfOrigin", channel)
                    .WithParameter("message", message)
                    .WithParameter("timestamp", timestamp)
                    .WithParameter("deletedBy", deletedBy);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when attempting to save chat message");
                throw;
            }
        }
    }
}
