using AntiHarassment.Chatlistener.Core.Events;
using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class ChatlogService : IChatlogService
    {
        private readonly ICompositeChatClient compositeChatClient;
        private readonly IChatRepository chatRepository;
        private readonly IDeletedMessagesRepository deletedMessagesRepository;
        private readonly IDatetimeProvider datetimeProvider;
        private readonly ILogger<ChatlogService> logger;

        public ChatlogService(
            ICompositeChatClient compositeChatClient,
            IChatRepository chatRepository,
            IDeletedMessagesRepository deletedMessagesRepository,
            IDatetimeProvider datetimeProvider,
            ILogger<ChatlogService> logger)
        {
            this.compositeChatClient = compositeChatClient;
            this.chatRepository = chatRepository;
            this.deletedMessagesRepository = deletedMessagesRepository;
            this.datetimeProvider = datetimeProvider;
            this.logger = logger;
        }

        public void Start()
        {
            logger.LogInformation("Starting Chatlogging");
            compositeChatClient.OnMessageReceived += Client_OnMessageReceived;
            logger.LogInformation("Chatlogging Initiated");
        }

        private async Task Client_OnMessageReceived(MessageReceivedEvent e)
        {
            if (e.Deleted)
            {
                await deletedMessagesRepository.Insert(e.Channel, e.DisplayName, e.DeletedBy, e.Message, datetimeProvider.UtcNow).ConfigureAwait(false);

                var existingChatMessage = await chatRepository.GetMessageFromTwitchMessageId(e.TwitchMessageId).ConfigureAwait(false);
                if (existingChatMessage == null)
                {
                    var chatMessage = new ChatMessage(
                    datetimeProvider.UtcNow,
                    e.TwitchMessageId,
                    e.DisplayName,
                    e.Channel,
                    e.Message,
                    e.AutoModded,
                    e.Deleted);

                    await chatRepository.SaveChatMessage(chatMessage).ConfigureAwait(false);

                    return;
                }

                existingChatMessage.MarkDeleted();

                await chatRepository.SaveChatMessage(existingChatMessage).ConfigureAwait(false);
            }
            else
            {
                var chatMessage = new ChatMessage(
                    datetimeProvider.UtcNow,
                    e.TwitchMessageId,
                    e.DisplayName,
                    e.Channel,
                    e.Message,
                    e.AutoModded,
                    e.Deleted);

                await chatRepository.SaveChatMessage(chatMessage).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            compositeChatClient.OnMessageReceived -= Client_OnMessageReceived;
        }
    }
}
