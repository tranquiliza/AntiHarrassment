using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Sql
{
    public class SuspensionRepository : ISuspensionRepository
    {
        private readonly ISqlAccess sql;
        private readonly ILogger<SuspensionRepository> logger;

        public SuspensionRepository(string connectionString, ILogger<SuspensionRepository> logger)
        {
            sql = SqlAccessBase.Create(connectionString);
            this.logger = logger;
        }

        public async Task<List<string>> GetSuspendedUsersForChannel(string channelName)
        {
            var result = new List<string>();
            using (var command = sql.CreateStoredProcedure("[Core].[GetSuspendedUsersForChannel]"))
            {
                command.WithParameter("channelOfOrigin", channelName);
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        result.Add(reader.GetString("username"));
                    }
                }
            }

            return result;
        }

        public async Task<Suspension> GetSuspension(Guid suspensionId)
        {
            try
            {
                using (var command = sql.CreateStoredProcedure("[Core].[GetSuspension]"))
                {
                    command.WithParameter("suspensionId", suspensionId);
                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await reader.ReadAsync().ConfigureAwait(false))
                            return Serialization.Deserialize<Suspension>(reader.GetString("data"));
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when getting suspension by id {}", suspensionId);
                throw;
            }
        }

        public async Task<List<Suspension>> GetSuspensionsForChannel(string channelOfOrigin)
        {
            try
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
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when getting suspensions for Channel {arg}", channelOfOrigin);
                throw;
            }
        }

        public async Task<List<Suspension>> GetSuspensionsForUser(string username)
        {
            try
            {
                var result = new List<Suspension>();
                using (var command = sql.CreateStoredProcedure("[Core].[GetSuspensionsForUser]"))
                {
                    command.WithParameter("username", username);
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
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when getting suspensions for user {arg}", username);
                throw;
            }
        }

        public async Task Save(Suspension suspension)
        {
            try
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
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when attempting to save suspension");
                throw;
            }
        }
    }
}
