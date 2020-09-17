using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Messaging.Commands;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Handlers
{
    public class TrainSystemCommandHandler : IHandleMessages<TrainSystemCommand>
    {
        private readonly IDataAnalyser dataAnalyser;
        private readonly ILogger<TrainSystemCommandHandler> logger;

        public TrainSystemCommandHandler(IDataAnalyser dataAnalyser, ILogger<TrainSystemCommandHandler> logger)
        {
            this.dataAnalyser = dataAnalyser;
            this.logger = logger;
        }

        public async Task Handle(TrainSystemCommand message, IMessageHandlerContext context)
        {
            logger.LogInformation("Received Train System... Doing a quick workout!");
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
