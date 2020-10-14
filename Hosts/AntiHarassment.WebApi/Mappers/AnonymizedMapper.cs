using AntiHarassment.Contract.Public;
using AntiHarassment.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace AntiHarassment.WebApi.Mappers
{
    public static class AnonymizedMapper
    {
        public static IEnumerable<AnonymizedSuspensionModel> MapAnon(this IEnumerable<Suspension> suspensions)
            => suspensions.Select(MapAnon);

        private static AnonymizedSuspensionModel MapAnon(this Suspension suspension)
        {
            return new AnonymizedSuspensionModel
            {
                TimestampUtc = suspension.Timestamp,
                DurationInSeconds = suspension.Duration,
                SuspensionId = suspension.SuspensionId,
                SuspensionSource = suspension.SuspensionSource.ToString(),
                SuspensionType = suspension.SuspensionType.ToString(),
                SystemReason = suspension.SystemReason,
                Tags = suspension.Tags.MapSimple(),
                ChatMessages = suspension.ChatMessages.MapAnon(),
            };
        }

        private static IEnumerable<AnonymizedChatMessageModel> MapAnon(this IEnumerable<ChatMessage> chatMessages)
            => chatMessages.Select(Map);

        private static AnonymizedChatMessageModel Map(this ChatMessage chatMessage)
        {
            return new AnonymizedChatMessageModel
            {
                AutoModded = chatMessage.AutoModded,
                Message = chatMessage.Message,
                TimestampUtc = chatMessage.Timestamp
            };
        }

        private static IEnumerable<SimpleTagModel> MapSimple(this IEnumerable<Tag> tags)
            => tags.Select(MapSimple);

        private static SimpleTagModel MapSimple(this Tag tag)
            => new SimpleTagModel
            {
                TagId = tag.TagId,
                TagName = tag.TagName
            };
    }
}
