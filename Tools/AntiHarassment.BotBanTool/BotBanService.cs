using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntiHarassment.Tool
{
    public class BotBanService
    {
        private readonly ISuspensionRepository suspensionRepository;
        private readonly ITagRepository tagRepository;

        public BotBanService(ISuspensionRepository suspensionRepository, ITagRepository tagRepository)
        {
            this.suspensionRepository = suspensionRepository;
            this.tagRepository = tagRepository;
        }

        public async Task CreateBansFor(List<string> usernames, string channelOfOrigin)
        {
            var utcNow = DateTime.UtcNow;

            var systemContext = new SystemAppContext();

            var botTag = await tagRepository.Get(Guid.Parse("FD77CBEE-10C0-43F5-8FDD-D30F9775520A")).ConfigureAwait(false);

            var chatMessage = new ChatMessage(
                utcNow,
                "",
                "",
                "",
                "",
                false,
                false);

            var fakedChatMessages = new List<ChatMessage> { chatMessage };

            foreach (var username in usernames)
            {
                var suspension = Suspension.CreateBan(username, channelOfOrigin, utcNow, fakedChatMessages, true);
                suspension.TryAddTag(botTag, systemContext, utcNow);

                foreach (var usernameOfBot in usernames)
                {
                    if (usernameOfBot == username)
                        continue;

                    suspension.AddUserLink(usernameOfBot, "Bots of same origin", systemContext, utcNow);
                }

                await suspensionRepository.Save(suspension).ConfigureAwait(false);
            }
        }
    }
}
