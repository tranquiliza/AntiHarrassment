using AntiHarassment.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiHarassment.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using AntiHarassment.Contract;

namespace AntiHarassment.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class SuspensionsController : ContextController
    {
        private readonly ISuspensionService suspensionService;

        public SuspensionsController(ISuspensionService suspensionService)
        {
            this.suspensionService = suspensionService;
        }

        [HttpGet("{channelOfOrigin}")]
        public async Task<IActionResult> GetSuspensionsForAll([FromRoute] string channelOfOrigin)
        {
            var result = await suspensionService.GetAllSuspensionsAsync(channelOfOrigin, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.Success)
                return Ok(result.Data.Map());

            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            return NoContent();
        }
    }
}
