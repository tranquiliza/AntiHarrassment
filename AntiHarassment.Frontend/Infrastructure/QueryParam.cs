using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Infrastructure
{
    public class QueryParam
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public QueryParam(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
