using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface ISystemReportService
    {
        Task<IResult<SystemReport>> GetSystemReport(IApplicationContext context);
    }
}
