using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public class UserReport
    {
        public string Username { get; private set; }
        public List<Suspension> Suspensions { get; private set; }

        public UserReport()
        {

        }
    }
}
