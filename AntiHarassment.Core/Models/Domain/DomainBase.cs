using AntiHarassment.Core.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AntiHarassment.Core.Models
{
    public abstract class DomainBase
    {
        [JsonProperty]
        public List<AuditTrail> Changes { get; private set; } = new List<AuditTrail>();

        protected void AddAuditTrail<T>(IApplicationContext context, string propertyName, T newValue, DateTime timeStamp)
        {
            var newValueJson = Serialization.Serialize(newValue);

            var newAuditTrail = new AuditTrail(context.UserId, context.User.TwitchUsername, propertyName, newValueJson, typeof(T).FullName, timeStamp);
            Changes.Add(newAuditTrail);
        }
    }
}
