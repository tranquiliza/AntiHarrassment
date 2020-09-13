using AntiHarassment.Contract;
using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AntiHarassment.WebApi.Mappers
{
    public static class SuspensionMapper
    {
        public static List<SuspensionModel> Map(this List<Suspension> suspensions, string urlBase)
            => suspensions.Select(x => x.Map(urlBase)).ToList();

        public static SuspensionModel Map(this Suspension suspension, string urlBase)
        {
            return new SuspensionModel
            {
                ChannelOfOrigin = suspension.ChannelOfOrigin,
                Duration = suspension.Duration,
                SuspensionId = suspension.SuspensionId,
                Timestamp = suspension.Timestamp,
                Username = suspension.Username,
                InvalidSuspension = suspension.InvalidSuspension,
                InvalidationReason = suspension.InvalidationReason,
                Audited = suspension.Audited,
                UnconfirmedSource = suspension.UnconfirmedSource,
                Tags = suspension.Tags.Map(),
                SuspensionType = suspension.SuspensionType.Map(),
                Messages = suspension.ChatMessages.Map(),
                LinkedUsers = suspension.LinkedUsers.Map(),
                SuspensionSource = suspension.SuspensionSource.Map(),
                Images = suspension.Images.Select(x => urlBase.TrimEnd('/') + "/images/" + x).ToList(),
                SystemReason = suspension.SystemReason
            };
        }

        private static List<LinkedUserModel> Map(this IReadOnlyList<LinkedUser> input)
            => input.Select(Map).ToList();

        private static LinkedUserModel Map(this LinkedUser input)
            => new LinkedUserModel
            {
                Username = input.Username,
                Reason = input.Reason
            };

        [SuppressMessage("General", "RCS1079:Throwing of new NotImplementedException.", Justification = "Can't implement future features before we're in the future.")]
        private static SuspensionTypeModel Map(this SuspensionType suspensionType)
            => suspensionType switch
            {
                SuspensionType.Timeout => SuspensionTypeModel.Timeout,
                SuspensionType.Ban => SuspensionTypeModel.Ban,
                _ => throw new NotImplementedException(),
            };

        [SuppressMessage("General", "RCS1079:Throwing of new NotImplementedException.", Justification = "Can't implement future features before we're in the future.")]
        private static SuspensionSourceModel Map(this SuspensionSource suspensionSource)
    => suspensionSource switch
    {
        SuspensionSource.Listener => SuspensionSourceModel.Listener,
        SuspensionSource.User => SuspensionSourceModel.User,
        SuspensionSource.System => SuspensionSourceModel.System,
        _ => throw new NotImplementedException()
    };
    }
}
