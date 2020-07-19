using AntiHarassment.Contract;
using AntiHarassment.Core;
using AntiHarassment.Core.Security;
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
    public class TagsController : ContextController
    {
        private readonly ITagService tagService;

        public TagsController(ITagService tagService)
        {
            this.tagService = tagService;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateOrUpdateTag([FromBody] UpdateTagModel model)
        {
            if (model.TagId != default)
            {
                var result = await tagService.Update(model.TagId, model.TagName, ApplicationContext).ConfigureAwait(false);
                if (result.State == ResultState.Success)
                    return Ok(result.Data.Map());

                return BadRequest(result.FailureReason);
            }
            else
            {
                var result = await tagService.Create(model.TagName, ApplicationContext).ConfigureAwait(false);
                if (result.State == ResultState.Success)
                    return Ok(result.Data.Map());

                return BadRequest(result.FailureReason);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTags([FromQuery] Guid tagId)
        {
            if (tagId != default)
            {
                var result = await tagService.Get(tagId).ConfigureAwait(false);
                if (result.State == ResultState.Success)
                    return Ok(result.Data.Map());
            }
            else
            {
                var result = await tagService.Get().ConfigureAwait(false);
                if (result.State == ResultState.Success)
                    return Ok(result.Data.Map());
            }

            return NoContent();
        }
    }
}
