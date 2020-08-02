using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface IChannelReportService
    {
        Task<IResult<ChannelReport>> GenerateReportForChannel(string channelName, IApplicationContext context);
    }
}