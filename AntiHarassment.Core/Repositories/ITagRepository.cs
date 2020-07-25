using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface ITagRepository
    {
        Task Save(Tag newTag);
        Task<Tag> Get(Guid tagId);
        Task<List<Tag>> Get();
        Task Delete(Guid tagId);
    }
}
