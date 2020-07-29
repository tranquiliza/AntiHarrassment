﻿using AntiHarassment.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiHarassment.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using AntiHarassment.Contract;
using AntiHarassment.Contract.Suspensions;

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

        [HttpGet]
        public async Task<IActionResult> GetSuspension([FromQuery] Guid suspensionId)
        {
            var result = await suspensionService.GetSuspensionAsync(suspensionId, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Failure)
                return BadRequest();

            if (result.State == ResultState.Success)
                return Ok(result.Data.Map());

            return NoContent();
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

        [HttpPost("{suspensionId}/validity")]
        public async Task<IActionResult> UpdateSuspensionValidity([FromRoute] Guid suspensionId, [FromBody] MarkSuspensionValidityModel model)
        {
            var result = await suspensionService.UpdateValidity(suspensionId, model.Invalidate, model.InvalidationReason, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.Failure)
                return BadRequest(result.FailureReason);

            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.Success)
                return Ok(result.Data.Map());

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
                return Ok(result.Data.Map());

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
                return Ok(result.Data.Map());

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
                return Ok(result.Data.Map());

            return NoContent();
        }
    }
}
