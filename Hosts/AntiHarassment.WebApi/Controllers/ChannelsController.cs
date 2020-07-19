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

        public ChannelsController(IChannelService channelService)
        {
            this.channelService = channelService;
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChannelModel model)
        {
            await channelService.UpdateChannel(model.ChannelName, model.ShouldListen, ApplicationContext).ConfigureAwait(false);

            return Ok();
        }
    }
}
