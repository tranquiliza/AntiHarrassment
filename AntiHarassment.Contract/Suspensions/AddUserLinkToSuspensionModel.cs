using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Contract
{
    public class AddUserLinkToSuspensionModel
    {
        public string Username { get; set; }
        public string LinkUserReason { get; set; }
    }
}
