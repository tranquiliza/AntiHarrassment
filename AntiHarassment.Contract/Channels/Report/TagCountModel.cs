using AntiHarassment.Contract.Tags;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Contract
{
    public class TagCountModel
    {
        public TagModel Tag { get; set; }
        public int Count { get; set; }
    }
}
