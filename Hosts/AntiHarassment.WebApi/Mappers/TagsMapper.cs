using AntiHarassment.Contract.Tags;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Mappers
{
    public static class TagsMapper
    {
        public static List<TagModel> Map(this List<Tag> tags)
            => tags.Select(Map).ToList();

        public static TagModel Map(this Tag tag)
            => new TagModel
            {
                TagId = tag.TagId,
                TagName = tag.TagName
            };
    }
}
