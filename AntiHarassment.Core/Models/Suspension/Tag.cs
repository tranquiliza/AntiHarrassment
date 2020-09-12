using AntiHarassment.Core.Security;
using Newtonsoft.Json;
using System;

namespace AntiHarassment.Core.Models
{
    public class Tag : DomainBase
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

        public void UpdateName(string newName, IApplicationContext context, DateTime timestamp)
        {
            if (TagName == newName)
                return;

            TagName = newName;

            AddAuditTrail(context, nameof(TagName), TagName, timestamp);
        }

        public void UpdateDescription(string description, IApplicationContext context, DateTime timestamp)
        {
            if (Description == description)
                return;

            Description = description;

            AddAuditTrail(context, nameof(Description), Description, timestamp);
        }
    }
}
