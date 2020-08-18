using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Core.Models
{
    public class AuditTrail
    {
        public Guid UserId { get; private set; }
        public string TwitchUsername { get; private set; }
        public string PropertyChanged { get; private set; }
        public string NewValue { get; private set; }
        public string TypeName { get; private set; }
        public DateTime TimeStamp { get; private set; }

        public AuditTrail(Guid userId, string twitchUsername, string propertyChanged, string newValue, string typeName, DateTime timeStamp)
        {
            UserId = userId;
            TwitchUsername = twitchUsername;
            PropertyChanged = propertyChanged;
            NewValue = newValue;
            TypeName = typeName;
            TimeStamp = timeStamp;
        }
    }
}
