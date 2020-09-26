using AntiHarassment.Chatlistener.Core;
using AntiHarassment.Core;
using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using AntiHarassment.MachineLearning.Models;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ML.DataOperationsCatalog;

namespace AntiHarassment.MachineLearning
{
    public class DataAnalyser : IDataAnalyser
    {
        private readonly ITagRepository tagRepository;
        private readonly ISuspensionRepository suspensionRepository;
        private readonly IDatetimeProvider datetimeProvider;
        private readonly string modelsStoragePath;
        private readonly ILogger<DataAnalyser> logger;
        private readonly IApplicationContext systemApplicationContext;
        private readonly Dictionary<Guid, PredictionEngine<SuspensionDataRow, SentimentPrediction>> predictionEngineLookup;
        private readonly List<Guid> skipTag;

        public DataAnalyser(string fileStoragePath, ITagRepository tagRepository, ISuspensionRepository suspensionRepository, IDatetimeProvider datetimeProvider, ILogger<DataAnalyser> logger)
        {
            this.tagRepository = tagRepository;
            this.suspensionRepository = suspensionRepository;
            this.datetimeProvider = datetimeProvider;
            this.logger = logger;
            this.systemApplicationContext = new SystemAppContext();
            this.modelsStoragePath = Path.Combine(fileStoragePath, "machineModels");
            this.predictionEngineLookup = new Dictionary<Guid, PredictionEngine<SuspensionDataRow, SentimentPrediction>>();
            this.skipTag = new List<Guid>();
        }

        /// <summary>
        /// Creates a binary predication model for every tag in the system.
        /// Run Daily to train better models every day!
        /// </summary>
        /// <returns></returns>
        public async Task TrainMachineLearningModels()
        {
            var tags = await GetSafeTags().ConfigureAwait(false);

            var suspensions = await GetValidAndAuditedSuspensions().ConfigureAwait(false);

            foreach (var tag in tags)
            {
                var datarows = suspensions.Select(suspension => new SuspensionDataRow(suspension, tag));

                var mlContext = new MLContext();

                IDataView dataView = mlContext.Data.LoadFromEnumerable(datarows);

                TrainTestData trainTestSplit = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.25);
                IDataView trainingData = trainTestSplit.TrainSet;
                IDataView testData = trainTestSplit.TestSet;

                // STEP 2: Common data process configuration with pipeline data transformations          
                var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SuspensionDataRow.Text));

                // STEP 3: Set the training algorithm, then create and config the modelBuilder                            
                var trainer = mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(SuspensionDataRow.IsFlaggedAsTag), featureColumnName: "Features");
                var trainingPipeline = dataProcessPipeline.Append(trainer);

                // STEP 4: Train the model fitting to the DataSet
                ITransformer trainedModel = trainingPipeline.Fit(trainingData);

                Directory.CreateDirectory(modelsStoragePath);
                var fullPathForSave = GetPathForTag(tag);

                mlContext.Model.Save(trainedModel, trainingData.Schema, fullPathForSave);
            }

            predictionEngineLookup.Clear();
            skipTag.Clear();
        }

        private async Task<IEnumerable<Suspension>> GetValidAndAuditedSuspensions()
        {
            var suspensions = await suspensionRepository.GetSuspensions(datetimeProvider.UtcNow.AddYears(-1)).ConfigureAwait(false);
            return suspensions.Where(x => x.Audited && !x.InvalidSuspension);
        }

        private string GetPathForTag(Tag tag)
        {
            return Path.Combine(modelsStoragePath, tag.TagId.ToString("N") + ".zip");
        }

        /// <summary>
        /// Run daily to check old unaudited if they can be tagged by the machine!
        /// </summary>
        /// <returns></returns>
        public async Task AttemptTagUnauditedSuspensions()
        {
            try
            {
                var suspensions = await suspensionRepository.GetSuspensions(datetimeProvider.UtcNow.AddYears(-1)).ConfigureAwait(false);
                foreach (var suspension in suspensions.Where(x => !x.Audited && !x.InvalidSuspension))
                {
                    var predicted = await PredictTagsForSuspension(suspension).ConfigureAwait(false);
                    if (predicted != null)
                        await suspensionRepository.Save(predicted).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Was unable to process something in the data analyser, this is the stack trace: {arg}", ex.Message);
            }
        }

        public async Task<Suspension> AttemptToTagSuspension(Suspension suspension)
        {
            var taggedSuspension = await PredictTagsForSuspension(suspension).ConfigureAwait(false);

            return taggedSuspension ?? suspension;
        }

        /// <summary>
        /// Returns NULL when no tags were added!
        /// </summary>
        /// <param name="suspension"></param>
        /// <returns></returns>
        private async Task<Suspension> PredictTagsForSuspension(Suspension suspension)
        {
            var mlContext = new MLContext();

            var tags = await GetSafeTags().ConfigureAwait(false);

            var hasBeenModified = false;

            foreach (var tag in tags)
            {
                // We skip because this previously failed, and we dont expect it to suddenly work.
                if (skipTag.Contains(tag.TagId))
                    continue;

                if (!predictionEngineLookup.TryGetValue(tag.TagId, out var engine))
                {
                    try
                    {
                        using Stream fileStream = new FileStream(GetPathForTag(tag), FileMode.Open);
                        var trainedModel = mlContext.Model.Load(fileStream, out var schema);

                        engine = mlContext.Model.CreatePredictionEngine<SuspensionDataRow, SentimentPrediction>(trainedModel);
                        predictionEngineLookup.Add(tag.TagId, engine);
                    }
                    catch (Exception ex)
                    {
                        skipTag.Add(tag.TagId);
                        logger.LogWarning("Unable to read predictionModel for {name} with id {id}, exception message: {message}", tag.TagName, tag.TagId, ex.ToString());
                        continue;
                    }
                }

                var testSubject = new SuspensionDataRow(suspension, tag);

                var tagPrediction = engine.Predict(testSubject);
                if (tagPrediction.Prediction && tagPrediction.Probability > 0.70)
                {
                    logger.LogInformation("Tagged {susId} with tag {tagName} from channel {channelName}", suspension.SuspensionId, tag.TagName, suspension.ChannelOfOrigin);
                    suspension.TryAddTag(tag, systemApplicationContext, datetimeProvider.UtcNow);
                    hasBeenModified = true;
                }
            }

            if (!hasBeenModified)
                return null;

            return suspension;
        }

        private async Task<List<Tag>> GetSafeTags()
        {
            var tags = await tagRepository.Get().ConfigureAwait(false);
            return tags.Where(x => x.TagId != Guid.Parse("B4FA9D56-BF4B-43CD-9562-791031B6BFA2")).ToList();
        }
    }
}
