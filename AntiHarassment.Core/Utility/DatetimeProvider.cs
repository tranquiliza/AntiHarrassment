using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Core
{
    public class DatetimeProvider : IDatetimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
