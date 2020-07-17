using AntiHarassment.Contract;
using AntiHarassment.Core.Models;
using NServiceBus.Persistence.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiHarassment.WebApi.Mappers;

namespace AntiHarassment.WebApi.Mappers
{
    public static class SuspensionMapper
    {
        public static List<SuspensionModel> Map(this List<Suspension> suspensions)
            => suspensions.Select(Map).ToList();

        public static SuspensionModel Map(this Suspension suspension)
        {
            return new SuspensionModel
            {
                ChannelOfOrigin = suspension.ChannelOfOrigin,
                Duration = suspension.Duration,
                SuspensionId = suspension.SuspensionId,
                Timestamp = suspension.Timestamp,
                Username = suspension.Username,
                InvalidSuspension = suspension.InvalidSuspension,
                Audited = suspension.Audited,
                Tags = suspension.Tags.Map(),
                SuspensionType = suspension.SuspensionType.Map(),
                Messages = suspension.ChatMessages.Map()
            };
        }

        private static SuspensionTypeModel Map(this SuspensionType suspensionType)
            => suspensionType switch
            {
                SuspensionType.Timeout => SuspensionTypeModel.Timeout,
                SuspensionType.Ban => SuspensionTypeModel.Ban,
                _ => throw new NotImplementedException(),
            };

        private static List<ChatMessageModel> Map(this List<ChatMessage> chatMessages)
            => chatMessages.Select(Map).ToList();

        private static ChatMessageModel Map(this ChatMessage chatMessage)
            => new ChatMessageModel
            {
                Message = chatMessage.Message,
                Timestamp = chatMessage.Timestamp
            };
    }
}
