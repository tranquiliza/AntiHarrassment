using AntiHarassment.Contract;
using AntiHarassment.Core;
using AntiHarassment.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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

            return Ok(result.Data);
        }

        [HttpPost("{channelName}/moderators")]
        public async Task<IActionResult> AddModerator([FromRoute] string channelName, [FromBody] AddModeratorModel model)
        {
            var result = await channelService.AddModeratorToChannel(channelName, model.ModeratorTwitchUsername, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.NoContent)
                return NoContent();

            return Ok(result.Data.Map());
        }

        [HttpDelete("{channelName}/moderators")]
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
            if (report.State == ResultState.AccessDenied)
                return Unauthorized();

            if (report.State == ResultState.Success)
                return Ok(report.Data.Map());

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChannelModel model)
        {
            await channelService.UpdateChannelListenerState(model.ChannelName, model.ShouldListen, ApplicationContext).ConfigureAwait(false);

            return Ok();
        }

        [HttpPost("{channelName}/systemIsModerator")]
        public async Task<IActionResult> SetSystemIsModeratorStatus([FromRoute] string channelName, [FromBody] UpdateSystemIsModeratorStatusModel model)
        {
            var result = await channelService.UpdateChannelSystemIsModeratorState(channelName, model.SystemIsModerator, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.NoContent)
                return BadRequest();

            return Ok(result.Data.Map());
        }

        [HttpPost("{channelName}/channelRules")]
        public async Task<IActionResult> AddRuleToChannel([FromRoute] string channelName, [FromBody] AddChannelRuleModel model)
        {
            var action = model.ChannelRuleAction.Map();

            var result = await channelService.AddRuleToChannel(channelName, model.RuleName, model.TagId, model.BansForTrigger, model.TimeoutsForTrigger, action, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.NoContent)
                return BadRequest();

            return Ok(result.Data.Map());
        }

        [HttpDelete("{channelName}/channelRules")]
        public async Task<IActionResult> RemoveRuleFromChannel([FromRoute] string channelName, [FromBody] DeleteChannelRuleModel model)
        {
            var result = await channelService.RemoveRuleFromChannel(channelName, model.RuleId, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.NoContent)
                return BadRequest();

            return Ok(result.Data.Map());
        }

        [HttpPost("{channelName}/channelRules/{ruleId}")]
        public async Task<IActionResult> UpdateRuleOnChannel([FromRoute] string channelName, [FromRoute] Guid ruleId, [FromBody] UpdateChannelRuleModel model)
        {
            var action = model.ChannelRuleAction.Map();
            var result = await channelService.UpdateRuleForChannel(channelName, ruleId, model.RuleName, model.BansForTrigger, model.TimeOutsForTrigger, action, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.NoContent)
                return BadRequest();

            return Ok(result.Data.Map());
        }

        [HttpGet("{channelName}/channelRules/exceeded")]
        public async Task<IActionResult> GetUsersWhoExceededChannelsRules([FromRoute] string channelName)
        {
            var result = await channelReportService.GetUsersWhoExceedsRules(channelName, ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.NoContent)
                return NoContent();

            return Ok(result.Data.Map());
        }

        [HttpPost("{channelName}/users/ruleCheck")]
        public async Task<IActionResult> SendBanCommandForUser([FromRoute] string channelName, [FromBody] ManuallyRunRuleCheckModel model)
        {
            await channelService.InitiateManualRuleCheck(channelName, model.TwitchUsername, ApplicationContext).ConfigureAwait(false);
            return Ok();
        }
    }
}
