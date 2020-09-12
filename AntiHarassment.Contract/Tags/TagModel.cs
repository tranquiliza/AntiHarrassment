using System;

namespace AntiHarassment.Contract.Tags
{
    public class TagModel
    {
        public Guid TagId { get; set; }
        public string TagName { get; set; }
        public string TagDescription { get; set; }
    }
}
