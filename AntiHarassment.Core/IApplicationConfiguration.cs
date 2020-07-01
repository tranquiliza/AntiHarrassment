using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Core
{
    public interface IApplicationConfiguration
    {
        string SecurityKey { get; }
    }
}
