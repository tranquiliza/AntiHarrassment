using AntiHarassment.Chatlistener.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Sql
{
    public class ChatRepository : IChatRepository
    {
        private readonly ISqlAccess sql;

        public ChatRepository(string connectionString)
        {
            sql = SqlAccessBase.Create(connectionString);
        }

        public async Task SaveChatMessage(string username, string channelOfOrigin, string message, DateTime timestamp)
        {
            using (var command = sql.CreateStoredProcedure("[Core].[InsertChatMessage]"))
            {
                command.WithParameter("username", username)
                    .WithParameter("channelOfOrigin", channelOfOrigin)
                    .WithParameter("message", message)
                    .WithParameter("timestamp", timestamp);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}
