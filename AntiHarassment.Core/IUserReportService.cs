using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface IUserReportService
    {
        Task<IResult<UserReport>> GetUserReportFor(string username);
        Task<IResult<List<string>>> GetUsersMatchedByTag(Guid tagId);
    }
}
