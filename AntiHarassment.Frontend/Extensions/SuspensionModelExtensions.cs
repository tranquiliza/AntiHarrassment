using AntiHarassment.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Contract
{
    public static class SuspensionModelExtensions
    {
        public static DateTime LocalTimeStamp(this SuspensionModel model)
        {
            return model.Timestamp.ToLocalTime();
        }
    }
}
