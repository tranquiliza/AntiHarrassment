namespace AntiHarassment.Chatlistener.Core.Events
{
    public class MessageReceivedEvent
    {
        public string Channel { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public bool AutoModded { get; set; }
    }
}