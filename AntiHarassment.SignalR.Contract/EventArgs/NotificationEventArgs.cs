namespace AntiHarassment.SignalR.Contract.EventArgs
{
    public class NotificationEventArgs
    {
        public string Username { get; set; }
        public string RuleName { get; set; }
        public string ChannelOfOrigin { get; set; }
    }
}
