﻿CREATE TABLE [Core].[Suspension]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [SuspensionId] UNIQUEIDENTIFIER NULL,
    [Username] NVARCHAR(100) NOT NULL, 
    [ChannelOfOrigin] NVARCHAR(100) NOT NULL, 
    [TypeOfSuspension] NVARCHAR(100) NOT NULL, 
    [Timestamp] DATETIME2(0) NOT NULL, 
    [Duration] INT NOT NULL, 
    [UnconfirmedSource] BIT NOT NULL DEFAULT 0,
    [Data] NVARCHAR(MAX) NOT NULL
)
GO

CREATE INDEX [IX_SUSPENSION_SUSPENSIONSID] ON [Core].[Suspension](SuspensionId)
GO

CREATE INDEX [IX_SUSPENSION_USERNAME] ON [Core].[Suspension](Username)
GO

CREATE INDEX [IX_SUSPENSION_CHANNELOFORIGIN] ON [Core].[Suspension](ChannelOfOrigin)
GO

CREATE INDEX [IX_SUSPENSION_UNCONFIRMEDSOURCE] ON [Core].[Suspension](UnconfirmedSource)
GO