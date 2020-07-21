﻿using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface ISuspensionRepository
    {
        Task Save(Suspension suspension);
        Task<List<Suspension>> GetSuspensionsForChannel(string channelOfOrigin);
        Task<Suspension> GetSuspension(Guid suspensionId);
        Task<List<Suspension>> GetSuspensionsForUser(string username);
    }
}
