namespace AntiHarassment.Messaging.Events
{
    public class AutoModListenerEnabledForChannelEvent
    {
        public string ChannelName { get; set; }

        public AutoModListenerEnabledForChannelEvent(string channelName)
        {
            ChannelName = channelName;
        }
    }
}
