using System;

namespace AntiHarassment.Messaging.Commands
{
    public class SendPasswordResetTokenCommand
    {
        public Guid UserId { get; set; }
    }
}
