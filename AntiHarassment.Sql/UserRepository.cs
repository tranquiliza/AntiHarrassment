using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Sql
{
    public class UserRepository : IUserRepository
    {
        private readonly ISqlAccess sql;
        private readonly ILogger<UserRepository> logger;

        public UserRepository(string connectionString, ILogger<UserRepository> logger)
        {
            sql = SqlAccessBase.Create(connectionString);
            this.logger = logger;
        }

        public async Task<User> GetByTwitchUsername(string twitchUsername)
        {
            try
            {
                using (var command = sql.CreateStoredProcedure("[Core].[GetUserByTwitchUsername]"))
                {
                    command.WithParameter("twitchUsername", twitchUsername);

                    using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                    {
                        if (await reader.ReadAsync().ConfigureAwait(false))
                            return Serialization.Deserialize<User>(reader.GetString("data"));
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when getting user by twitchUsername");
                throw;
            }
        }

        public async Task<User> GetById(Guid id)
        {
            try
            {
                using (var command = sql.CreateStoredProcedure("[Core].[GetUserById]"))
                {
                    command.WithParameter("userId", id);
                    using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                    {
                        if (await reader.ReadAsync().ConfigureAwait(false))
                            return Serialization.Deserialize<User>(reader.GetString("data"));
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when getting user by id");
                throw;
            }
        }

        public async Task Save(User user)
        {
            try
            {
                using (var command = sql.CreateStoredProcedure("[Core].[InsertUpdateUser]"))
                {
                    command.WithParameter("userId", user.Id)
                        .WithParameter("twitchUsername", user.TwitchUsername)
                        .WithParameter("email", user.Email)
                        .WithParameter("data", Serialization.Serialize(user));

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when saving user");
                throw;
            }
        }

        public async Task Delete(Guid userId)
        {
            try
            {
                using (var command = sql.CreateStoredProcedure("[Core].[DeleteUserById]"))
                {
                    command.WithParameter("userId", userId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when marking user deleted");
                throw;
            }
        }
    }
}
