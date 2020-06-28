namespace AntiHarassment.Chatlistener.Core.Events
{
    public class UserBannedEvent
    {
        public string BanReason { get; set; }
        public string Channel { get; set; }
        public string Username { get; set; }
    }
}