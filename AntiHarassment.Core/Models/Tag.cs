using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public class Tag
    {
        [JsonProperty]
        public Guid TagId { get; private set; }

        [JsonProperty]
        public string TagName { get; private set; }

        private Tag() { }

        public Tag(string tagName)
        {
            TagId = Guid.NewGuid();
            TagName = tagName;
        }

        public void UpdateName(string newName)
        {
            TagName = newName;
        }
    }
}
