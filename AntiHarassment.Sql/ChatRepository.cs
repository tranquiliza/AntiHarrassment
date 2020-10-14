using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Sql
{
    public class ChatRepository : IChatRepository
    {
        private readonly ISqlAccess sql;
        private readonly ILogger<ChatRepository> logger;

        public ChatRepository(string connectionString, ILogger<ChatRepository> logger)
        {
            sql = SqlAccessBase.Create(connectionString);
            this.logger = logger;
        }

        public async Task<DateTime> GetTimeStampForLatestMessage()
        {
            using (var command = sql.CreateStoredProcedure("[Core].[GetLatestMessageTimestamp]"))
            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                if (await reader.ReadAsync().ConfigureAwait(false))
                    return reader.GetDateTime("Timestamp");
            }

            return default;
        }

        public async Task<List<ChatMessage>> GetMessagesFor(string username, string channelOfOrigin, TimeSpan chatRecordWindow, DateTime timeOfSuspension)
        {
            try
            {
                var earliestTime = timeOfSuspension.Subtract(chatRecordWindow);

                var result = new List<ChatMessage>();

                using (var command = sql.CreateStoredProcedure("[Core].[GetChatMessagesForUser]"))
                {
                    command.WithParameter("username", username)
                        .WithParameter("channelOfOrigin", channelOfOrigin)
                        .WithParameter("earliestTime", earliestTime);

                    using var reader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.Default).ConfigureAwait(false);
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        result.Add(Serialization.Deserialize<ChatMessage>(reader.GetString("data")));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when getting messages for Username on Channel (Suspension ChatLog)");
                throw;
            }
        }

        public async Task<List<ChatMessage>> GetMessagesForChannel(string channelOfOrigin, DateTime earliestTime, DateTime latestTime)
        {
            try
            {
                var result = new List<ChatMessage>();
                using (var command = sql.CreateStoredProcedure("[Core].[GetChatLogForChannel]"))
                {
                    command.WithParameter("channelOfOrigin", channelOfOrigin)
                        .WithParameter("earliestTime", earliestTime)
                        .WithParameter("latestTime", latestTime);

                    using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        result.Add(Serialization.Deserialize<ChatMessage>(reader.GetString("data")));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when getting messages for channel");
                throw;
            }
        }

        public async Task<List<string>> GetUniqueChattersForChannel(string channelOfOrigin)
        {
            try
            {
                var result = new List<string>();
                using (var command = sql.CreateStoredProcedure("[Core].[GetUniqueChattersForChannel]"))
                {
                    command.WithParameter("channelOfOrigin", channelOfOrigin);
                    using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    while (await reader.ReadAsync().ConfigureAwait(false))
                        result.Add(reader.GetString("Username"));
                }

                using (var command = sql.CreateStoredProcedure("[Core].[GetUniqueUsersFromSuspensions]"))
                {
                    command.WithParameter("channelOfOrigin", channelOfOrigin);
                    using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var value = reader.GetString("username");
                        if (!result.Contains(value))
                            result.Add(value);
                    }
                }

                return result.Distinct().ToList();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when fetching distinct users for channel");
                throw;
            }
        }

        public async Task<List<string>> GetUniqueChattersForSystem()
        {
            try
            {
                var result = new List<string>(500000);
                using (var command = sql.CreateStoredProcedure("[Core].[GetUniqueChattersForSystem]"))
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                        result.Add(reader.GetString("Username"));
                }

                using (var command = sql.CreateStoredProcedure("[Core].[GetUniqueUsersFromSuspensionsForSystem]"))
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                        result.Add(reader.GetString("username"));
                }

                using (var command = sql.CreateStoredProcedure("[Core].[GetAllChatters]"))
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                        result.Add(reader.GetString("TwitchUsername"));
                }

                return result.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when fetching distinct users for channel");
                throw;
            }
        }

        public async Task SaveChatMessage(ChatMessage chatMessage)
        {
            try
            {
                using var command = sql.CreateStoredProcedure("[Core].[UpsertChatMessage]");
                command.WithParameter("chatMessageID", chatMessage.ChatMessageId)
                    .WithParameter("username", chatMessage.Username)
                    .WithParameter("twitchMessageId", chatMessage.TwitchMessageId)
                    .WithParameter("channelOfOrigin", chatMessage.ChannelOfOrigin)
                    .WithParameter("automodded", chatMessage.AutoModded)
                    .WithParameter("deleted", chatMessage.Deleted)
                    .WithParameter("message", chatMessage.Message)
                    .WithParameter("timestamp", chatMessage.Timestamp)
                    .WithParameter("data", Serialization.Serialize(chatMessage));

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when attempting to save chat message");
                throw;
            }
        }

        public async Task<ChatMessage> GetMessageFromTwitchMessageId(string twitchMessageId)
        {
            try
            {
                using var command = sql.CreateStoredProcedure("[Core].[GetChatMessageFromTwitchMessageId]");
                command.WithParameter("twitchMessageId", twitchMessageId);

                using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                if (await reader.ReadAsync().ConfigureAwait(false))
                    return Serialization.Deserialize<ChatMessage>(reader.GetString("data"));

                return null;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when attempting to fetch a chat message from Twitch Message Id");
                throw;
            }
        }

        // TODO REMOVE AFTER 2.0.0
        /// <summary>
        /// PLEASE DELETE ME AFTER UPDATE COMPLETED TO 2.0.0 (THANKS)
        /// </summary>
        /// <returns></returns>
        public async Task MigrateData()
        {
            using var command = sql.CreateQuery(@"SELECT [Id]
      ,[ChatMessageId]
      ,[TwitchMessageId]
      ,[Username]
      ,[ChannelOfOrigin]
      ,[Message]
      ,[Timestamp]
      ,[AutoModded]
      ,[Deleted]
      ,[Data]
  FROM [Core].[ChatMessage]
  WHERE [ChatMessageId] IS NULL");
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var chatMessage = new ChatMessage(
                    reader.GetDateTime("Timestamp"),
                    twitchMessageId: "",
                    reader.GetString("Username"),
                    reader.GetString("ChannelOfOrigin"),
                    reader.GetString("Message"),
                    reader.GetBoolean("AutoModded"),
                    reader.GetBoolean("Deleted"));

                using var updateCommand = sql.CreateQuery(@"UPDATE [Core].[ChatMessage]
  SET
  [ChatMessageId] = @chatMessageId,
  [TwitchMessageId] = '',
  [Data] = @data
  WHERE Id = @id");

                var id = reader.GetInt64("Id");
                Console.WriteLine($"Updateing row {id} to new data format!");

                updateCommand.WithParameter("chatMessageId", chatMessage.ChatMessageId)
                    .WithParameter("data", Serialization.Serialize(chatMessage))
                    .WithParameter("id", id);

                await updateCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}
