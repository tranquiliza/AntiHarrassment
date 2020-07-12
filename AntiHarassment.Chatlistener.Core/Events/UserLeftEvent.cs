namespace AntiHarassment.Chatlistener.Core.Events
{
    public class UserLeftEvent
    {
        public string Channel { get; set; }
        public string Username { get; set; }
    }
}