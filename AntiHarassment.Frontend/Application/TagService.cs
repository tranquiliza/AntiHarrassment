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

        public async Task AddNewTag(string tagName)
        {
            if (!userService.IsUserAdmin)
                return;

            var newTag = await apiGateway.Post<TagModel, UpdateTagModel>(new UpdateTagModel { TagName = tagName }, "tags").ConfigureAwait(false);
            if (newTag != null)
                Tags.Add(newTag);

            NotifyStateChanged();
        }
    }
}
