using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Sql
{
    public class TagRepository : ITagRepository
    {
        private readonly ISqlAccess sql;
        private readonly ILogger<TagRepository> logger;

        public TagRepository(string connectionString, ILogger<TagRepository> logger)
        {
            sql = SqlAccessBase.Create(connectionString);
            this.logger = logger;
        }

        public async Task Delete(Guid tagId)
        {
            try
            {
                using (var command = sql.CreateStoredProcedure("[Core].[DeleteTag]"))
                {
                    command.WithParameter("tagId", tagId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when attempting to delete tag with id {tagId}", tagId);
                throw;
            }
        }

        public async Task<Tag> Get(Guid tagId)
        {
            try
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
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when attempting to get tag with id {tagId}", tagId);
                throw;
            }
        }

        public async Task<List<Tag>> Get()
        {
            try
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
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when getting all tags");
                throw;
            }
        }

        public async Task Save(Tag tag)
        {
            try
            {
                using (var command = sql.CreateStoredProcedure("[Core].[UpsertTag]"))
                {
                    command.WithParameter("tagId", tag.TagId)
                        .WithParameter("tagName", tag.TagName)
                        .WithParameter("data", Serialization.Serialize(tag));

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error when attempting to save tag with id: {arg}", tag.TagId);
                throw;
            }
        }
    }
}
