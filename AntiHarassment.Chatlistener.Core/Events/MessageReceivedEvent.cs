namespace AntiHarassment.Chatlistener.Core.Events
{
    public class MessageReceivedEvent
    {
        public string TwitchMessageId { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
        public string DisplayName { get; set; }
        public bool AutoModded { get; set; }
        public bool Deleted { get; set; }

        /// <summary>
        /// Only available if Deleted is True
        /// </summary>
        public string DeletedBy { get; set; }
    }
}