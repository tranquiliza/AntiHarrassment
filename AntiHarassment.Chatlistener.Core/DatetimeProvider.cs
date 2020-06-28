using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Chatlistener.Core
{
    public class DatetimeProvider : IDatetimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
