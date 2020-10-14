using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;
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
        Task<IResult<User>> Authenticate(string accessToken);
        Task<IResult> EnableDiscordForUser(Guid userId, ulong discordUserId, IApplicationContext context);
        Task<IResult> DisableDiscordForUser(Guid userId, IApplicationContext context);
        Task<IResult<User>> Get(Guid userId, IApplicationContext context);
    }
}
