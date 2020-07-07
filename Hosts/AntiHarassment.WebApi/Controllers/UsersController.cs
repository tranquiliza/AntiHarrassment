﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AntiHarassment.Contract;
using AntiHarassment.Core;
using AntiHarassment.WebApi.Mappers;
using AntiHarassment.Core.Security;

namespace AntiHarassment.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
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
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateModel authenticateModel)
        {
            var result = await userService.Authenticate(authenticateModel.TwitchUsername, authenticateModel.Password).ConfigureAwait(false);
            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(applicationConfiguration.SecurityKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, result.Data.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var roleClaims = result.Data.Roles.Select(role => new Claim(ClaimTypes.Role, role));
            if (roleClaims != null)
                tokenDescriptor.Subject.AddClaims(roleClaims);
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var bearerToken = tokenHandler.WriteToken(token);

            return Ok(result.Data.Map(bearerToken));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody]RegisterUserModel registerUserModel)
        {
            var result = await userService.Create(registerUserModel.Email, registerUserModel.TwitchUsername, registerUserModel.Password).ConfigureAwait(false);
            if (result.State != ResultState.Success)
                return BadRequest(result.FailureReason);

            return Ok(result.Data.Map());
        }
    }
}