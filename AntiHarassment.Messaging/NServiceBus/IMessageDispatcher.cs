using System;
using System.Threading.Tasks;

namespace AntiHarassment.Messaging.NServiceBus
{
    public interface IMessageDispatcher
    {
        /// <summary>
        /// Publish events.
        /// </summary>
        /// <param name="message">Event to send</param>
        /// <returns>Awaitable task</returns>
        Task Publish<T>(T message);

        /// <summary>
        /// Send messages and commands
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Awaitable task</returns>
        Task Send<T>(T message);

        /// <summary>
        /// Send message/command on local queue.
        /// </summary>
        /// <param name="message">Message/command to send</param>
        /// <returns>Awaitable task</returns>
        Task SendLocal<T>(T message);

        /// <summary>
        /// Send message, but delay it for later delivery
        /// </summary>
        /// <param name="message">Message/command to send</param>
        /// <param name="deliveryDelay">Delay actual delivery of the message with this timespan</param>
        /// <returns>Awaitable task</returns>
        Task SendDelayed<T>(T message, TimeSpan deliveryDelay);
    }
}
