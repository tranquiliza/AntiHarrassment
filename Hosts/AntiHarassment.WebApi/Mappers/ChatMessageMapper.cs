﻿using AntiHarassment.Contract;
using AntiHarassment.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace AntiHarassment.WebApi.Mappers
{
    public static class ChatMessageMapper
    {
        public static List<ChatMessageModel> Map(this List<ChatMessage> chatMessages)
            => chatMessages.Select(Map).ToList();

        public static ChatMessageModel Map(this ChatMessage chatMessage)
            => new ChatMessageModel
            {
                Username = chatMessage.Username,
                Message = chatMessage.Message,
                Timestamp = chatMessage.Timestamp,
                AutoModded = chatMessage.AutoModded,
                Deleted = chatMessage.Deleted
            };
    }
}
