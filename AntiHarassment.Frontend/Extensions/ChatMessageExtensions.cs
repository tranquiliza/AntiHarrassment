using AntiHarassment.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Contract
{
    public static class ChatMessageExtensions
    {
        public static DateTime LocalTimestamp(this ChatMessageModel model)
        {
            return model.Timestamp.ToLocalTime();
        }
    }
}
