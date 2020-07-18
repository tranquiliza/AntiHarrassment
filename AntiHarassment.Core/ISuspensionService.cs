using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface ISuspensionService
    {
        Task<IResult<List<Suspension>>> GetAllSuspensionsAsync(string channelOfOrigin, IApplicationContext context);
        Task<IResult<Suspension>> UpdateValidity(Guid suspensionId, bool invalidate, IApplicationContext context);
        Task<IResult<Suspension>> UpdateAuditState(Guid suspensionId, bool audited, IApplicationContext applicationContext);
        Task<IResult<Suspension>> AddTagTo(Guid suspensionId, Guid tagId, IApplicationContext applicationContext);
        Task<IResult<Suspension>> RemoveTagFrom(Guid suspensionId, Guid tagId, IApplicationContext applicationContext);
        Task<IResult<Suspension>> GetSuspensionAsync(Guid suspensionId, IApplicationContext applicationContext);
    }
}
