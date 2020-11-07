namespace AntiHarassment.Chatlistener.Core.Events
{
    public class UserBannedEvent
    {
        public EventSource Source { get; set; }
        public string BanReason { get; set; }
        public string Channel { get; set; }
        public string Username { get; set; }
        public string BannedBy { get; set; }
    }
}