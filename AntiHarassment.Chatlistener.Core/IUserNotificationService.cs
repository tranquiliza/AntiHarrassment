using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IUserNotificationService
    {
        Task SendConfirmationTokenToUser(Guid userId);
        Task SendPassworkResetTokenToUser(Guid userId);
    }
}
