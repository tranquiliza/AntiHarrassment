namespace AntiHarassment.Chatlistener.Core.Events
{
    public class UserJoinedEvent
    {
        public string Channel { get; set; }
        public string Username { get; set; }
    }
}