using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Sql
{
    public class ChatRepository : IChatRepository
    {
        private readonly ISqlAccess sql;

        public ChatRepository(string connectionString)
        {
            sql = SqlAccessBase.Create(connectionString);
        }

        public async Task<List<ChatMessage>> GetMessagesFor(string username, string channelOfOrigin, TimeSpan chatRecordWindow, DateTime timeOfSuspension)
        {
            var earliestTime = timeOfSuspension.Subtract(chatRecordWindow);

            var result = new List<ChatMessage>();

            using (var command = sql.CreateStoredProcedure("[Core].[GetChatMessagesForUser]"))
            {
                command.WithParameter("username", username)
                    .WithParameter("channelOfOrigin", channelOfOrigin)
                    .WithParameter("earliestTime", earliestTime);

                using (var reader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.Default).ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var chatMessage = new ChatMessage(reader.GetDateTime("timestamp"), reader.GetString("message"));
                        result.Add(chatMessage);
                    }
                }
            }

            return result;
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
