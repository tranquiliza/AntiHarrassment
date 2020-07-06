using AntiHarassment.Core;
using AntiHarassment.Core.Models;
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

        public UserRepository(string connectionString)
        {
            sql = SqlAccessBase.Create(connectionString);
        }

        public async Task<User> GetByTwitchUsername(string twitchUsername)
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

        public async Task Save(User user)
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
    }
}
