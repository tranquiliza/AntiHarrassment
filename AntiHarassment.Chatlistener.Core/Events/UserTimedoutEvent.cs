namespace AntiHarassment.Chatlistener.Core.Events
{
    public class UserTimedoutEvent
    {
        public string Channel;

        /// <summary>
        /// This is in seconds
        /// </summary>
        public int TimeoutDuration;
        public string TimeoutReason;
        public string Username;
    }
}