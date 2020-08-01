using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiHarassment.Contract;
using AntiHarassment.Core;
using AntiHarassment.Core.Security;
using AntiHarassment.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AntiHarassment.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ChannelsController : ContextController
    {
        private readonly IChannelService channelService;
        private readonly IChannelReportService channelReportService;

        public ChannelsController(IChannelService channelService, IChannelReportService channelReportService)
        {
            this.channelService = channelService;
            this.channelReportService = channelReportService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string channelName)
        {
            if (!string.IsNullOrEmpty(channelName))
            {
                var singleChannelResult = await channelService.GetChannel(channelName, ApplicationContext).ConfigureAwait(false);
                if (singleChannelResult.State == ResultState.AccessDenied)
                    return Unauthorized();

                if (singleChannelResult.State == ResultState.NoContent)
                    return NoContent();

                return Ok(singleChannelResult.Data.Map());
            }
            else
            {
                var multipleChannelsResult = await channelService.GetChannels(ApplicationContext).ConfigureAwait(false);
                if (multipleChannelsResult.State == ResultState.AccessDenied)
                    return Unauthorized();

                if (multipleChannelsResult.State == ResultState.NoContent)
                    return NoContent();

                return Ok(multipleChannelsResult.Data.Map());
            }
        }

        [HttpGet("{channelName}/chatlogs")]
        public async Task<IActionResult> GetChatLogForChannel([FromRoute] string channelName, [FromQuery] DateTime earliestTime, [FromQuery] DateTime latestTime)
        {
            var result = await channelService.GetChatLogs(channelName, earliestTime, latestTime, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.NoContent)
                return NoContent();

            return Ok(result.Data.Map());
        }

        [HttpGet("{channelName}/users")]
        public async Task<IActionResult> GetSeenUsersForChannel([FromRoute] string channelName)
        {
            var result = await channelService.GetChattersForChannel(channelName, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.NoContent)
                return NoContent();

            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            // No mapping needed, its a list of strings
            return Ok(result.Data);
        }

        [HttpPost("{channelName}")]
        public async Task<IActionResult> AddModerator([FromRoute] string channelName, [FromBody] AddModeratorModel model)
        {
            var result = await channelService.AddModeratorToChannel(channelName, model.ModeratorTwitchUsername, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.NoContent)
                return NoContent();

            return Ok(result.Data.Map());
        }

        [HttpDelete("{channelName}")]
        public async Task<IActionResult> DeleteModerator([FromRoute] string channelName, [FromBody] DeleteModeratorModel model)
        {
            var result = await channelService.DeleteModeratorFromChannel(channelName, model.ModeratorTwitchUsername, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.NoContent)
                return NoContent();

            return Ok(result.Data.Map());
        }

        [HttpGet("{channelName}/report")]
        public async Task<IActionResult> GetStatisticsForChannel([FromRoute] string channelName)
        {
            var report = await channelReportService.GenerateReportForChannel(channelName, ApplicationContext).ConfigureAwait(false);
            if (report.State == ResultState.Success)
                return Ok(report.Data.Map());

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChannelModel model)
        {
            await channelService.UpdateChannel(model.ChannelName, model.ShouldListen, ApplicationContext).ConfigureAwait(false);

            return Ok();
        }
    }
}
