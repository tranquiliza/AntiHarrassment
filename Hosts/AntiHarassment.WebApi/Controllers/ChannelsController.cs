﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiHarassment.Contract;
using AntiHarassment.Core;
using AntiHarassment.WebApi.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AntiHarassment.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelsController : ControllerBase
    {
        private readonly IChannelService channelService;

        public ChannelsController(IChannelService channelService)
        {
            this.channelService = channelService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var channels = await channelService.GetChannels().ConfigureAwait(false);

            return Ok(channels.Map());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ChannelModel model)
        {
            await channelService.UpdateChannel(model.ChannelName, model.ShouldListen).ConfigureAwait(false);

            return Ok();
        }
    }
}
