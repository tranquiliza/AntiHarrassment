﻿using AntiHarassment.Contract.Tags;
using AntiHarassment.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace AntiHarassment.WebApi.Mappers
{
    public static class TagsMapper
    {
        public static List<TagModel> Map(this IReadOnlyList<Tag> tags)
            => tags.ToList().Map();

        public static List<TagModel> Map(this List<Tag> tags)
            => tags.Select(Map).ToList();

        public static TagModel Map(this Tag tag)
            => new TagModel
            {
                TagId = tag.TagId,
                TagName = tag.TagName,
                TagDescription = tag.Description
            };
    }
}
