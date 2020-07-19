using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Sql
{
    public class TagRepository : ITagRepository
    {
        private readonly ISqlAccess sql;

        public TagRepository(string connectionString)
        {
            sql = SqlAccessBase.Create(connectionString);
        }

        public async Task<Tag> Get(Guid tagId)
        {
            using (var command = sql.CreateStoredProcedure("[Core].[GetTag]"))
            {
                command.WithParameter("tagId", tagId);
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                        return Serialization.Deserialize<Tag>(reader.GetString("data"));
                }
            }

            return null;
        }

        public async Task<List<Tag>> Get()
        {
            var result = new List<Tag>();

            using (var command = sql.CreateStoredProcedure("[Core].[GetTags]"))
            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                    result.Add(Serialization.Deserialize<Tag>(reader.GetString("data")));
            }

            return result;
        }

        public async Task Save(Tag tag)
        {
            using (var command = sql.CreateStoredProcedure("[Core].[UpsertTag]"))
            {
                command.WithParameter("tagId", tag.TagId)
                    .WithParameter("tagName", tag.TagName)
                    .WithParameter("data", Serialization.Serialize(tag));

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}
