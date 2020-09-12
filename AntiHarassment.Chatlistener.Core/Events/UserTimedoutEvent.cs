namespace AntiHarassment.Chatlistener.Core.Events
{
    public class UserTimedoutEvent
    {
        public string Channel { get; set; }

        /// <summary>
        /// This is in seconds
        /// </summary>
        public int TimeoutDuration { get; set; }
        public string TimeoutReason { get; set; }
        public string Username { get; set; }
        public string TimedoutBy { get; set; }
    }
}