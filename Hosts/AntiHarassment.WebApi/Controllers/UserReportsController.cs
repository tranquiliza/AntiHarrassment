using AntiHarassment.Core;
using AntiHarassment.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserReportsController : ContextController
    {
        private readonly IUserReportService userReportService;

        public UserReportsController(IUserReportService userReportService)
        {
            this.userReportService = userReportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReport([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Please provide a username");

            var result = await userReportService.GetUserReportFor(username).ConfigureAwait(false);
            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            if (result.State == ResultState.Success)
                return Ok(result.Data.Map(CurrentUrl));

            return NoContent();
        }

        [HttpGet("tag")]
        public async Task<IActionResult> GetUsersByTag([FromQuery] Guid tagId)
        {
            var result = await userReportService.GetUsersMatchedByTag(tagId).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Success)
                return Ok(result.Data);

            return NoContent();
        }
    }
}
