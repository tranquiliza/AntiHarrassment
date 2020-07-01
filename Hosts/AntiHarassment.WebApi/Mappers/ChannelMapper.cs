using AntiHarassment.Contract;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Mappers
{
    public static class ChannelMapper
    {
        public static List<ChannelModel> Map(this List<Channel> channels)
        {
            return channels.Select(Map).ToList();
        }

        public static ChannelModel Map(this Channel channel)
        {
            return new ChannelModel
            {
                ChannelName = channel.ChannelName,
                ShouldListen = channel.ShouldListen
            };
        }
    }
}
