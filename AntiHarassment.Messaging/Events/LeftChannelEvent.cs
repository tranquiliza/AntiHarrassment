namespace AntiHarassment.Messaging.Events
{
    public class LeftChannelEvent
    {
        public string ChannelName { get; set; }

        public LeftChannelEvent(string channelName)
        {
            ChannelName = channelName;
        }
    }
}
