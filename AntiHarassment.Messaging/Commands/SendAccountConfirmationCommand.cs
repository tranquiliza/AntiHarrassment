using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Messaging.Commands
{
    public class SendAccountConfirmationCommand
    {
        public Guid UserId { get; set; }
    }
}
