using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Messaging.NServiceBus
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly IMessageSession messageSession;

        public MessageDispatcher(IMessageSession messageSession)
        {
            this.messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
        }

        public Task Publish<T>(T message) => messageSession.Publish(message);

        public Task Send<T>(T message) => messageSession.Send(message);

        public Task SendLocal<T>(T message) => messageSession.SendLocal(message);

        public Task SendDelayed<T>(T message, TimeSpan deliveryDelay)
        {
            var options = new SendOptions();
            options.DelayDeliveryWith(deliveryDelay);
            return messageSession.Send(message, options);
        }
    }
}
