namespace AntiHarassment.Core.Models
{
    public interface ITwitchAccessTokenResult
    {
        string TwitchUsername { get; set; }
        string Email { get; set; }
    }
}
