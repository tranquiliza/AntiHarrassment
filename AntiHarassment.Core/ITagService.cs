using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface ITagService
    {
        Task<IResult<Tag>> Update(Guid tagId, string tagName, string description, IApplicationContext context);
        Task<IResult<Tag>> Create(string tagName, string description, IApplicationContext context);
        Task<IResult<Tag>> Get(Guid tagId);
        Task<IResult<List<Tag>>> Get();
        Task<IResult> Delete(Guid tagId, IApplicationContext applicationContext);
    }
}
