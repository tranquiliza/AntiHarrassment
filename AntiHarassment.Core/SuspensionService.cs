using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using AntiHarassment.Messaging.Events;
using AntiHarassment.Messaging.NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public class SuspensionService : ISuspensionService
    {
        private readonly ISuspensionRepository suspensionRepository;
        private readonly IChannelRepository channelRepository;
        private readonly ITagRepository tagRepository;
        private readonly IMessageDispatcher messageDispatcher;
        private readonly IDatetimeProvider datetimeProvider;
        private readonly IFileRepository fileRepository;

        public SuspensionService(
            ISuspensionRepository suspensionRepository,
            IChannelRepository channelRepository,
            ITagRepository tagRepository,
            IMessageDispatcher messageDispatcher,
            IDatetimeProvider datetimeProvider,
            IFileRepository fileRepository)
        {
            this.suspensionRepository = suspensionRepository;
            this.channelRepository = channelRepository;
            this.tagRepository = tagRepository;
            this.messageDispatcher = messageDispatcher;
            this.datetimeProvider = datetimeProvider;
            this.fileRepository = fileRepository;
        }

        public async Task<IResult<List<Suspension>>> GetAllSuspensionsAsync(string channelOfOrigin, IApplicationContext context)
        {
            var channel = await channelRepository.GetChannel(channelOfOrigin).ConfigureAwait(false);
            if (channel == null)
                return Result<List<Suspension>>.NoContentFound();

            if (!context.HaveAccessTo(channel))
                return Result<List<Suspension>>.Unauthorized();

            var dataForUser = await suspensionRepository.GetSuspensionsForChannel(channelOfOrigin).ConfigureAwait(false);
            if (dataForUser.Count > 0)
                return Result<List<Suspension>>.Succeeded(dataForUser.OrderByDescending(x => x.Timestamp).ToList());

            return Result<List<Suspension>>.NoContentFound();
        }

        public async Task<IResult<Suspension>> RemoveTagFrom(Guid suspensionId, Guid tagId, IApplicationContext context)
        {
            var fetch = await RetrieveSuspensionAndCheckAccess(suspensionId, context).ConfigureAwait(false);
            if (fetch.State != ResultState.Success)
                return fetch;

            var suspension = fetch.Data;

            var tag = await tagRepository.Get(tagId).ConfigureAwait(false);
            suspension.RemoveTag(tag, context, datetimeProvider.UtcNow);
            await suspensionRepository.Save(suspension).ConfigureAwait(false);

            await PublishSuspensionUpdatedEvent(suspension).ConfigureAwait(false);

            return Result<Suspension>.Succeeded(suspension);
        }

        public async Task<IResult<Suspension>> AddTagTo(Guid suspensionId, Guid tagId, IApplicationContext context)
        {
            var fetch = await RetrieveSuspensionAndCheckAccess(suspensionId, context).ConfigureAwait(false);
            if (fetch.State != ResultState.Success)
                return fetch;

            var suspension = fetch.Data;
            var tag = await tagRepository.Get(tagId).ConfigureAwait(false);

            if (!suspension.TryAddTag(tag, context, datetimeProvider.UtcNow))
                return Result<Suspension>.Succeeded(suspension);

            await suspensionRepository.Save(suspension).ConfigureAwait(false);

            await PublishSuspensionUpdatedEvent(suspension).ConfigureAwait(false);

            return Result<Suspension>.Succeeded(suspension);
        }

        public async Task<IResult<Suspension>> UpdateAuditState(Guid suspensionId, bool audited, IApplicationContext context)
        {
            var fetch = await RetrieveSuspensionAndCheckAccess(suspensionId, context).ConfigureAwait(false);
            if (fetch.State != ResultState.Success)
                return fetch;

            var suspension = fetch.Data;
            suspension.UpdateAuditedState(audited, context, datetimeProvider.UtcNow);
            await suspensionRepository.Save(suspension).ConfigureAwait(false);

            await PublishSuspensionUpdatedEvent(suspension).ConfigureAwait(false);

            return Result<Suspension>.Succeeded(suspension);
        }

        public async Task<IResult<Suspension>> UpdateValidity(Guid suspensionId, bool invalidate, string invalidationReason, IApplicationContext context)
        {
            var fetch = await RetrieveSuspensionAndCheckAccess(suspensionId, context).ConfigureAwait(false);
            if (fetch.State != ResultState.Success)
                return fetch;

            var suspension = fetch.Data;
            if (!suspension.UpdateValidity(invalidate, invalidationReason, context, datetimeProvider.UtcNow))
                return Result<Suspension>.Failure("Cannot mark Invalid without a reason");

            await suspensionRepository.Save(suspension).ConfigureAwait(false);
            await PublishSuspensionUpdatedEvent(suspension).ConfigureAwait(false);

            return Result<Suspension>.Succeeded(suspension);
        }

        public async Task<IResult<Suspension>> AddUserLinkToSuspension(Guid suspensionId, string twitchUsername, IApplicationContext context)
        {
            var fetch = await RetrieveSuspensionAndCheckAccess(suspensionId, context).ConfigureAwait(false);
            if (fetch.State != ResultState.Success)
                return fetch;

            var suspension = fetch.Data;
            suspension.AddUserLink(twitchUsername, context, datetimeProvider.UtcNow);

            await suspensionRepository.Save(suspension).ConfigureAwait(false);
            await PublishSuspensionUpdatedEvent(suspension).ConfigureAwait(false);

            return Result<Suspension>.Succeeded(suspension);
        }

        public async Task<IResult<Suspension>> RemoveUserLinkFromSuspension(Guid suspensionId, string twitchUsername, IApplicationContext context)
        {
            var fetch = await RetrieveSuspensionAndCheckAccess(suspensionId, context).ConfigureAwait(false);
            if (fetch.State != ResultState.Success)
                return fetch;

            var suspension = fetch.Data;
            suspension.RemoveUserLink(twitchUsername, context, datetimeProvider.UtcNow);

            await suspensionRepository.Save(suspension).ConfigureAwait(false);
            await PublishSuspensionUpdatedEvent(suspension).ConfigureAwait(false);

            return Result<Suspension>.Succeeded(suspension);
        }

        private async Task<IResult<Suspension>> RetrieveSuspensionAndCheckAccess(Guid suspensionId, IApplicationContext context)
        {
            var suspension = await suspensionRepository.GetSuspension(suspensionId).ConfigureAwait(false);
            if (suspension == null)
                return Result<Suspension>.Failure($"Unable to find suspension with id: {suspensionId}");

            var channel = await channelRepository.GetChannel(suspension.ChannelOfOrigin).ConfigureAwait(false);
            if (channel == null)
                return Result<Suspension>.Failure($"The channel {suspension.ChannelOfOrigin}, was not found");

            if (!context.HaveAccessTo(channel))
                return Result<Suspension>.Unauthorized();

            return Result<Suspension>.Succeeded(suspension);
        }

        public async Task<IResult<Suspension>> CreateManualSuspension(string username, string channelOfOrigin, IApplicationContext context)
        {
            var channel = await channelRepository.GetChannel(channelOfOrigin).ConfigureAwait(false);
            if (!context.HaveAccessTo(channel))
                return Result<Suspension>.Unauthorized();

            var newSuspension = Suspension.CreateManualBan(username, channelOfOrigin, datetimeProvider.UtcNow, context.User.TwitchUsername);
            await suspensionRepository.Save(newSuspension).ConfigureAwait(false);

            await messageDispatcher.Publish(new NewSuspensionEvent { SuspensionId = newSuspension.SuspensionId, ChannelOfOrigin = channelOfOrigin }).ConfigureAwait(false);

            return Result<Suspension>.Succeeded(newSuspension);
        }

        private async Task PublishSuspensionUpdatedEvent(Suspension suspension)
        {
            await messageDispatcher.Publish(new SuspensionUpdatedEvent { ChannelOfOrigin = suspension.ChannelOfOrigin, SuspensionId = suspension.SuspensionId }).ConfigureAwait(false);
        }

        public async Task<IResult<Suspension>> GetSuspensionAsync(Guid suspensionId, IApplicationContext context)
        {
            var suspension = await suspensionRepository.GetSuspension(suspensionId).ConfigureAwait(false);
            if (suspension == null)
                return Result<Suspension>.Failure("No suspension with given Id");

            var channelOfOrigin = await channelRepository.GetChannel(suspension.ChannelOfOrigin).ConfigureAwait(false);
            if (channelOfOrigin == null)
                return Result<Suspension>.Failure("Unable to fetch channel of origin");

            if (!context.HaveAccessTo(channelOfOrigin))
                return Result<Suspension>.Unauthorized();

            return Result<Suspension>.Succeeded(suspension);
        }

        public async Task AddImageTo(Guid suspensionId, byte[] imageBytes, string fileExtension, IApplicationContext context)
        {
            var suspension = await suspensionRepository.GetSuspension(suspensionId).ConfigureAwait(false);
            var channel = await channelRepository.GetChannel(suspension.ChannelOfOrigin).ConfigureAwait(false);
            if (!context.HaveAccessTo(channel))
                return;

            var imageName = suspension.AddImage(fileExtension, context, datetimeProvider.UtcNow);

            await suspensionRepository.Save(suspension).ConfigureAwait(false);
            await fileRepository.SaveImage(imageBytes, imageName).ConfigureAwait(false);

            await PublishSuspensionUpdatedEvent(suspension).ConfigureAwait(false);
        }
    }
}
