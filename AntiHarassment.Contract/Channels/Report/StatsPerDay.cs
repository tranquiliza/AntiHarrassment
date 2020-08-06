using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Contract
{
    public class StatsPerDay
    {
        public DateTime Date { get; set; }
        public int SuspensionsCount { get; set; }
        public int BansCount { get; set; }
        public int TimeoutCount { get; set; }
    }
}
