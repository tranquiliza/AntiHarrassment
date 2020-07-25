using AntiHarassment.Contract;
using AntiHarassment.Contract.Tags;
using AntiHarassment.Frontend.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public class TagService : ITagService
    {
        public List<TagModel> Tags { get; private set; }

        private readonly IApiGateway apiGateway;
        private readonly IUserService userService;

        public TagService(IApiGateway apiGateway, IUserService userService)
        {
            this.apiGateway = apiGateway;
            this.userService = userService;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
        public event Action OnChange;

        public async Task Initialize()
        {
            Tags = await apiGateway.Get<List<TagModel>>("tags").ConfigureAwait(false) ?? new List<TagModel>();
            NotifyStateChanged();
        }

        public async Task UpdateTag(Guid tagId, string tagName, string tagDescription)
        {
            if (!userService.IsUserAdmin)
                return;

            var model = new UpdateTagModel { TagId = tagId, TagName = tagName, TagDescription = tagDescription };

            var updatedTag = await apiGateway.Post<TagModel, UpdateTagModel>(model, "tags").ConfigureAwait(false);
            if (updatedTag != null)
            {
                var existingTag = Tags.Find(x => x.TagId == tagId);
                if (existingTag != null)
                    Tags.Remove(existingTag);

                Tags.Add(updatedTag);

                NotifyStateChanged();
            }
        }

        public async Task DeleteTag(Guid tagId)
        {
            await apiGateway.Delete("tags", routeValues: new string[] { tagId.ToString() }).ConfigureAwait(false);

            var existingTag = Tags.Find(x => x.TagId == tagId);
            if (existingTag != null)
                Tags.Remove(existingTag);

            NotifyStateChanged();
        }

        public async Task AddNewTag(string tagName, string tagDescription)
        {
            if (!userService.IsUserAdmin)
                return;

            var newTag = await apiGateway.Post<TagModel, UpdateTagModel>(new UpdateTagModel { TagName = tagName, TagDescription = tagDescription }, "tags").ConfigureAwait(false);
            if (newTag != null)
                Tags.Add(newTag);

            NotifyStateChanged();
        }
    }
}
