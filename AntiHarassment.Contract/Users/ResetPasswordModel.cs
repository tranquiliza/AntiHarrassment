using System;

namespace AntiHarassment.Contract
{
    public class ResetPasswordModel
    {
        public Guid ResetToken { get; set; }
        public string TwitchUsername { get; set; }
        public string NewPassword { get; set; }
    }
}
