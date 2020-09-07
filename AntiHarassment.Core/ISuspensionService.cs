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
        Task<IResult<List<Suspension>>> GetAllSuspensionsAsync(string channelOfOrigin, DateTime date, IApplicationContext context);
        Task<IResult<Suspension>> UpdateValidity(Guid suspensionId, bool invalidate, string invalidationReason, IApplicationContext context);
        Task<IResult<Suspension>> UpdateAuditState(Guid suspensionId, bool audited, IApplicationContext context);
        Task<IResult<Suspension>> AddTagTo(Guid suspensionId, Guid tagId, IApplicationContext context);
        Task<IResult<Suspension>> RemoveTagFrom(Guid suspensionId, Guid tagId, IApplicationContext context);
        Task<IResult<Suspension>> GetSuspensionAsync(Guid suspensionId, IApplicationContext context);
        Task<IResult<Suspension>> AddUserLinkToSuspension(Guid suspensionId, string twitchUsername, string linkReason, IApplicationContext context);
        Task<IResult<Suspension>> RemoveUserLinkFromSuspension(Guid suspensionId, string twitchUsername, IApplicationContext context);
        Task<IResult<Suspension>> CreateManualSuspension(string username, string channelOfOrigin, IApplicationContext context);
        Task AddImageTo(Guid suspensionId, byte[] imageBytes, string fileExtension, IApplicationContext context);
        Task<IResult<List<DateTime>>> GetUnauditedDatesFor(string channelOfOrigin, IApplicationContext context);
    }
}
