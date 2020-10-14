using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Core;
using AntiHarassment.Messaging.Commands;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class TrainSystemCommandHandler : IHandleMessages<TrainSystemCommand>
    {
        private readonly IDataAnalyser dataAnalyser;
        private readonly ILogger<TrainSystemCommandHandler> logger;
        private readonly IDiscordMessageClient discordMessageClient;

        public TrainSystemCommandHandler(IDataAnalyser dataAnalyser, ILogger<TrainSystemCommandHandler> logger, IDiscordMessageClient discordMessageClient)
        {
            this.dataAnalyser = dataAnalyser;
            this.logger = logger;
            this.discordMessageClient = discordMessageClient;
        }

        public async Task Handle(TrainSystemCommand message, IMessageHandlerContext context)
        {
            const string logMessage = "Received Train System event... Doing a quick workout! -> We shall be smarter!";

            logger.LogInformation(logMessage);
            await discordMessageClient.SendMessageToPrometheus(logMessage).ConfigureAwait(false);
            await dataAnalyser.TrainMachineLearningModels().ConfigureAwait(false);
            await dataAnalyser.AttemptTagUnauditedSuspensions().ConfigureAwait(false);

            var options = new SendOptions();
            options.DelayDeliveryWith(TimeSpan.FromDays(1));
            options.RouteToThisEndpoint();

            await context.Send(new TrainSystemCommand(), options).ConfigureAwait(false);
            logger.LogInformation("Delayed Train System command sent");
        }
    }
}
