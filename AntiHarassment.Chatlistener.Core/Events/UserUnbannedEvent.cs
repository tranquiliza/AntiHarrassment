namespace AntiHarassment.Chatlistener.Core.Events
{
    public class UserUnbannedEvent
    {
        public string Username { get; set; }
        public string UnbannedBy { get; set; }
        public string Channel { get; set; }
    }
}
