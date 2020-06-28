using System.Collections.Generic;

namespace AntiHarassment.Chatlistener.Core.Events
{
    public class ExistingUsersDetectedEvent
    {
        public List<string> Users { get; set; }
        public string Channel { get; set; }
    }
}