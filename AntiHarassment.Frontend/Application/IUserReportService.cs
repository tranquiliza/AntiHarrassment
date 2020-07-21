using AntiHarassment.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public interface IUserReportService
    {
        event Action OnChange;
        UserReportModel UserReport { get; }
        string UserReportLookupError { get; }

        Task SearchForUser(string username);
    }
}
