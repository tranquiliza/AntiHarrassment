using AntiHarassment.Contract;
using AntiHarassment.Core;
using AntiHarassment.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

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

        [HttpGet("unconfirmed")]
        public async Task<IActionResult> GetUnconfirmedSourcesSuspensions()
        {
            var result = await suspensionService.GetAllUnconfirmedSourcesSuspensions(ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Success)
                return Ok(result.Data.Map(CurrentUrl));

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetSuspension([FromQuery] Guid suspensionId)
        {
            var result = await suspensionService.GetSuspensionAsync(suspensionId, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Failure)
                return BadRequest();

            if (result.State == ResultState.Success)
                return Ok(result.Data.Map(CurrentUrl));

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSuspension([FromBody] CreateSuspensionModel model)
        {
            var result = await suspensionService.CreateManualSuspension(model.TwitchUsername, model.ChannelOfOrigin, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            return Ok(result.Data.Map(CurrentUrl));
        }

        [HttpGet("{channelOfOrigin}")]
        public async Task<IActionResult> GetSuspensionsForAll([FromRoute] string channelOfOrigin)
        {
            var result = await suspensionService.GetAllSuspensionsAsync(channelOfOrigin, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.Success)
                return Ok(result.Data.Map(CurrentUrl));

            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            return NoContent();
        }

        [HttpGet("{channelOfOrigin}/unauditedDates")]
        public async Task<IActionResult> GetDatesWithUnauditedSuspensions([FromRoute] string channelOfOrigin)
        {
            var result = await suspensionService.GetUnauditedDatesFor(channelOfOrigin, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            if (result.State == ResultState.Success)
                return Ok(result.Data);

            return NoContent();
        }

        [HttpPost("{suspensionId}/validity")]
        public async Task<IActionResult> UpdateSuspensionValidity([FromRoute] Guid suspensionId, [FromBody] MarkSuspensionValidityModel model)
        {
            var result = await suspensionService.UpdateValidity(suspensionId, model.Invalidate, model.InvalidationReason, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Success)
                return Ok(result.Data.Map(CurrentUrl));

            return NoContent();
        }

        [HttpPost("{suspensionId}/audit")]
        public async Task<IActionResult> UpdateSuspensionAuditStatus([FromRoute] Guid suspensionId, [FromBody] UpdateSuspensionAuditStateModel model)
        {
            var result = await suspensionService.UpdateAuditState(suspensionId, model.Audited, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Success)
                return Ok(result.Data.Map(CurrentUrl));

            return NoContent();
        }

        [HttpPost("{suspensionId}/tags")]
        public async Task<IActionResult> AddTagToSuspensions([FromRoute] Guid suspensionId, [FromBody] AddTagToSuspensionModel model)
        {
            var result = await suspensionService.AddTagTo(suspensionId, model.TagId, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Success)
                return Ok(result.Data.Map(CurrentUrl));

            return NoContent();
        }

        [HttpDelete("{suspensionId}/tags")]
        public async Task<IActionResult> DeleteTagFromSuspension([FromRoute] Guid suspensionId, [FromBody] DeleteTagFromSuspensionModel model)
        {
            var result = await suspensionService.RemoveTagFrom(suspensionId, model.TagId, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Success)
                return Ok(result.Data.Map(CurrentUrl));

            return NoContent();
        }

        [HttpPost("{suspensionId}/userlink")]
        public async Task<IActionResult> AddUserLinkToSuspension([FromRoute] Guid suspensionId, [FromBody] AddUserLinkToSuspensionModel model)
        {
            var result = await suspensionService.AddUserLinkToSuspension(suspensionId, model.Username, model.LinkUserReason, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Success)
                return Ok(result.Data.Map(CurrentUrl));

            return NoContent();
        }

        [HttpDelete("{suspensionId}/userlink")]
        public async Task<IActionResult> DeleteUserLinkFromSuspension([FromRoute] Guid suspensionId, [FromBody] DeleteUserlinkFromSuspensionModel model)
        {
            var result = await suspensionService.RemoveUserLinkFromSuspension(suspensionId, model.Username, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Success)
                return Ok(result.Data.Map(CurrentUrl));

            return NoContent();
        }

        [HttpPost("{suspensionId}/image")]
        public async Task<IActionResult> UploadImageForSuspension([FromRoute] Guid suspensionId, [FromForm] IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                var stream = file.OpenReadStream();
                await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
                await suspensionService.AddImageTo(suspensionId, memoryStream.ToArray(), Path.GetExtension(file.FileName), ApplicationContext).ConfigureAwait(false);
            }

            return Ok();
        }
    }
}
