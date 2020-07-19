using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Contract
{
    public class UpdateTagModel
    {
        public Guid TagId { get; set; }
        public string TagName { get; set; }
    }
}
