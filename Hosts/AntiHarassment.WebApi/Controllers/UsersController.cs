using AntiHarassment.Contract;
using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using AntiHarassment.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ContextController
    {
        private readonly IUserService userService;
        private readonly IApplicationConfiguration applicationConfiguration;

        public UsersController(IUserService userService, IApplicationConfiguration applicationConfiguration)
        {
            this.userService = userService;
            this.applicationConfiguration = applicationConfiguration;
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel authenticateModel)
        {
            var result = await userService.Authenticate(authenticateModel.TwitchUsername, authenticateModel.Password).ConfigureAwait(false);
            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            var bearerToken = GenerateToken(result);

            return Ok(result.Data.Map(bearerToken));
        }

        [AllowAnonymous]
        [HttpPost("AuthenticateWithTwitch")]
        public async Task<IActionResult> AuthenticateWithTwitch([FromBody] AuthenticateWithTwitchModel model)
        {
            var result = await userService.Authenticate(model.AccessToken).ConfigureAwait(false);
            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            var bearerToken = GenerateToken(result);

            return Ok(result.Data.Map(bearerToken));
        }

        private string GenerateToken(IResult<User> result)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(applicationConfiguration.SecurityKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, result.Data.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var roleClaims = result.Data.Roles.Select(role => new Claim(ClaimTypes.Role, role));
            if (roleClaims != null)
                tokenDescriptor.Subject.AddClaims(roleClaims);

            tokenDescriptor.Subject.AddClaim(new Claim(CustomClaimTypes.TwitchUsername, result.Data.TwitchUsername));

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid userId)
        {
            var result = await userService.Get(userId, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.NoContent)
                return BadRequest("No user with this Id?");

            return Ok(result.Data.Map());
        }

        [HttpPost("{userId}/discord")]
        public async Task<IActionResult> EnableDiscordNotifications([FromRoute] Guid userId, [FromBody] EnableDiscordForUserModel model)
        {
            var result = await userService.EnableDiscordForUser(userId, model.DiscordUserId, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            return Ok();
        }

        [HttpDelete("{userId}/discord")]
        public async Task<IActionResult> DeleteDiscordNotifications([FromRoute] Guid userId)
        {
            var result = await userService.DisableDiscordForUser(userId, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserModel registerUserModel)
        {
            var result = await userService.Create(registerUserModel.Email, registerUserModel.TwitchUsername, registerUserModel.Password).ConfigureAwait(false);
            if (result.State != ResultState.Success)
                return BadRequest(result.FailureReason);

            return Ok(result.Data.Map());
        }

        [HttpPost("requestResetPasswordToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestResetToken([FromBody] RequestResetTokenModel model)
        {
            var result = await userService.SendPasswordResetTokenFor(model.TwitchUsername).ConfigureAwait(false);
            if (result.State != ResultState.Success)
                return BadRequest(result.FailureReason);

            return Ok();
        }

        [HttpPost("UpdatePassword")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdatePassword([FromBody] ResetPasswordModel model)
        {
            var result = await userService.UpdatePasswordFor(model.TwitchUsername, model.ResetToken, model.NewPassword).ConfigureAwait(false);
            if (result.State != ResultState.Success)
                return BadRequest(result.FailureReason);

            return Ok();
        }

        [HttpPost("confirm/{twitchUsername}")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmToken([FromRoute] string twitchUsername, [FromBody] ConfirmUserModel model)
        {
            var result = await userService.Confirm(twitchUsername, model.ConfirmationToken).ConfigureAwait(false);
            if (result.State != ResultState.Success)
                return BadRequest(result.FailureReason);

            return Ok();
        }
    }
}
