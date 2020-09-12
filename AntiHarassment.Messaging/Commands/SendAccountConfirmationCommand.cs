using System;

namespace AntiHarassment.Messaging.Commands
{
    public class SendAccountConfirmationCommand
    {
        public Guid UserId { get; set; }
    }
}
