using System;

namespace AntiHarassment.Core
{
    public class DatetimeProvider : IDatetimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
