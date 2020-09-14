namespace AntiHarassment.Chatlistener.Core.Events
{
    public class MessageDeletedEvent
    {
        public string DeletedBy { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }
        public string Channel { get; set; }
    }
}
