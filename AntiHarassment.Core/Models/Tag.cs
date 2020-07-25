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

        [JsonProperty]
        public string Description { get; private set; }

        private Tag() { }

        public Tag(string tagName, string description)
        {
            TagId = Guid.NewGuid();
            TagName = tagName;
            Description = description;
        }

        public void UpdateName(string newName)
        {
            TagName = newName;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }
    }
}
