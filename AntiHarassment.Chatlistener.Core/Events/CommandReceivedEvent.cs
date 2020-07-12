using System.Collections.Generic;

namespace AntiHarassment.Chatlistener.Core.Events
{
    public class CommandReceivedEvent
    {
        public string CommandText { get; set; }
        public char CommandIdentifier { get; set; }
        public string ArgumentsAsString { get; set; }
        public List<string> ArgumentsAsList { get; set; }
        public string Channel { get; set; }
        public bool IsModerator { get; set; }
        public bool IsSubscriber { get; set; }
        public bool IsBroadcaster { get; set; }
        public string Username { get; set; }
        public UserType UserType { get; set; }
    }
}
