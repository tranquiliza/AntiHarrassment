namespace AntiHarassment.Chatlistener.Core.Events
{
    public class UserUntimedoutEvent
    {
        public string Username { get; set; }
        public string UntimedoutBy { get; set; }
        public string Channel { get; set; }
    }
}
