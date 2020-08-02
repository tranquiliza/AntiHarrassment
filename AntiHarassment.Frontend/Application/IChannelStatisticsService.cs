﻿using AntiHarassment.Contract;
using System;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public interface IChannelStatisticsService
    {
        event Action OnChange;
        ChannelReportModel ChannelReportModel { get; set; }
        string CurrentlySelectedChannel { get; set; }

        Task Initialize();
        Task ChangeChannel(string selectedChannel);
    }
}