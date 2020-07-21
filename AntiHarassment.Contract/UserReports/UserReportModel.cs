﻿using AntiHarassment.Contract.Tags;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Contract
{
    public class UserReportModel
    {
        public string Username { get; set; }
        public List<SuspensionModel> Suspensions { get; set; }
        public List<TagModel> Tags { get; set; }
        public List<string> BannedFromChannels { get; set; }
        public List<string> TimedoutFromChannels { get; set; }
    }
}