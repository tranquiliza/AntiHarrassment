using AntiHarassment.Chatlistener.Core.Events;
using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public class ChatlogService : IChatlogService
    {
        private readonly ICompositeChatClient compositeChatClient;
        private readonly IChatRepository chatRepository;
        private readonly IDeletedMessagesRepository deletedMessagesRepository;
        private readonly IDatetimeProvider datetimeProvider;

        public ChatlogService(
            ICompositeChatClient compositeChatClient,
            IChatRepository chatRepository,
            IDeletedMessagesRepository deletedMessagesRepository,
            IDatetimeProvider datetimeProvider)
        {
            this.compositeChatClient = compositeChatClient;

            this.compositeChatClient.OnMessageReceived += Client_OnMessageReceived;
            this.chatRepository = chatRepository;
            this.deletedMessagesRepository = deletedMessagesRepository;
            this.datetimeProvider = datetimeProvider;
        }

        private async Task Client_OnMessageReceived(MessageReceivedEvent e)
        {
            if (e.Deleted)
            {
                await deletedMessagesRepository.Insert(e.Channel, e.DisplayName, e.DeletedBy, e.Message, datetimeProvider.UtcNow).ConfigureAwait(false);

                var existingChatMessage = await chatRepository.GetMessageFromTwitchMessageId(e.TwitchMessageId).ConfigureAwait(false);
                if (existingChatMessage == null)
                    return;

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
    }
}
