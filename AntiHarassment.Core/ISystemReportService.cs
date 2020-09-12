using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface ISystemReportService
    {
        Task<IResult<SystemReport>> GetSystemReport(IApplicationContext context);
    }
}
