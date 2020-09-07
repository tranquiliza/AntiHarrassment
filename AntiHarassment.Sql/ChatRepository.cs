﻿using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                    using (var reader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.Default).ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var chatMessage = new ChatMessage(reader.GetDateTime("timestamp"), reader.GetString("message"), reader.GetBoolean("AutoModded"));
                            result.Add(chatMessage);
                        }
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

                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var chatMessage = new ChatMessage(reader.GetDateTime("timestamp"), reader.GetString("message"), reader.GetBoolean("AutoModded"));
                            chatMessage.AttachUsername(reader.GetString("username"));

                            result.Add(chatMessage);
                        }
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
                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                            result.Add(reader.GetString("Username"));
                    }
                }

                using (var command = sql.CreateStoredProcedure("[Core].[GetUniqueUsersFromSuspensions]"))
                {
                    command.WithParameter("channelOfOrigin", channelOfOrigin);
                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var value = reader.GetString("username");
                            if (!result.Contains(value))
                                result.Add(value);
                        }
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
                var result = new List<string>();
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
                    {
                        var value = reader.GetString("username");
                        if (!result.Contains(value))
                            result.Add(value);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when fetching distinct users for channel");
                throw;
            }
        }

        public async Task SaveChatMessage(string username, string channelOfOrigin, bool autoModded, string message, DateTime timestamp)
        {
            try
            {
                using (var command = sql.CreateStoredProcedure("[Core].[InsertChatMessage]"))
                {
                    command.WithParameter("username", username)
                        .WithParameter("channelOfOrigin", channelOfOrigin)
                        .WithParameter("message", message)
                        .WithParameter("automodded", autoModded)
                        .WithParameter("timestamp", timestamp);

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when attempting to save chat message");
                throw;
            }
        }
    }
}
