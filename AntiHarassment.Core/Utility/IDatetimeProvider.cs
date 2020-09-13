using System;

namespace AntiHarassment.Core
{
    public interface IDatetimeProvider
    {
        DateTime UtcNow { get; }
    }
}
