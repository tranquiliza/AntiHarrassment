namespace AntiHarassment.Messaging.Events
{
    public class JoinedChannelEvent
    {
        public string ChannelName { get; set; }

        public JoinedChannelEvent(string channelName)
        {
            ChannelName = channelName;
        }
    }
}
