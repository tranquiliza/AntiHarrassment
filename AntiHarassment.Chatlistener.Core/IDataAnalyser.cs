using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Chatlistener.Core
{
    public interface IDataAnalyser
    {
        Task TrainMachineLearningModels();
        Task AttemptTagUnauditedSuspensions();
        Task<Suspension> AttemptToTagSuspension(Suspension suspension);
    }
}
