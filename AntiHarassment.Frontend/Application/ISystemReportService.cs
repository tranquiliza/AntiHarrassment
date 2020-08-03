using AntiHarassment.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public interface ISystemReportService
    {
        event Action OnChange;
        SystemReportModel SystemReportModel { get; }

        Task Initialize();
    }
}
