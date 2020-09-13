using System;

namespace AntiHarassment.Contract
{
    public class UpdateTagModel
    {
        public Guid TagId { get; set; }
        public string TagName { get; set; }
        public string TagDescription { get; set; }
    }
}
