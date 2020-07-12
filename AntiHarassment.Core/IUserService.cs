using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface IUserService
    {
        Task<IResult<User>> Create(string email, string twitchUsername, string password, string roleName = null);
        Task<IResult<User>> Authenticate(string username, string password);
        Task<IResult> Confirm(string twitchUsername, Guid confirmationToken);
        Task<IResult> SendPasswordResetTokenFor(string twitchUsername);
        Task<IResult> UpdatePasswordFor(string twitchUsername, Guid resetToken, string newPassword);
    }
}
