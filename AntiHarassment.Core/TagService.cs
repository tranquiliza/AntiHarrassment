﻿using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public class TagService : ITagService
    {
        private readonly ITagRepository tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        public async Task<IResult<Tag>> Create(string tagName, IApplicationContext context)
        {
            if (!context.User.HasRole(Roles.Admin))
                return Result<Tag>.Unauthorized();

            var newTag = new Tag(tagName);
            await tagRepository.Save(newTag).ConfigureAwait(false);
            return Result<Tag>.Succeeded(newTag);
        }

        public async Task<IResult<Tag>> Get(Guid tagId)
        {
            var tag = await tagRepository.Get(tagId).ConfigureAwait(false);
            if (tag == null)
                return Result<Tag>.NoContentFound();

            return Result<Tag>.Succeeded(tag);
        }

        public async Task<IResult<List<Tag>>> Get()
        {
            var tags = await tagRepository.Get().ConfigureAwait(false);
            if (tags.Count == 0)
                return Result<List<Tag>>.NoContentFound();

            return Result<List<Tag>>.Succeeded(tags);
        }

        public async Task<IResult<Tag>> Update(Guid tagId, string tagName, IApplicationContext context)
        {
            if (!context.User.HasRole(Roles.Admin))
                return Result<Tag>.Unauthorized();

            var existingTag = await tagRepository.Get(tagId).ConfigureAwait(false);
            if (existingTag == null)
                return Result<Tag>.Failure("Tag not found, invalid Id");

            existingTag.UpdateName(tagName);
            await tagRepository.Save(existingTag).ConfigureAwait(false);

            return Result<Tag>.Succeeded(existingTag);
        }
    }
}
