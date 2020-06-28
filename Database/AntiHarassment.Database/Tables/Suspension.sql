CREATE TABLE [Core].[Suspension]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Username] NVARCHAR(100) NOT NULL, 
    [ChannelOfOrigin] NVARCHAR(100) NOT NULL, 
    [TypeOfSuspension] VARCHAR(100) NOT NULL, 
    [Timestamp] DATETIME2(0) NOT NULL, 
    [Duration] INT NOT NULL DEFAULT 0
)
GO

CREATE INDEX [IX_SUSPENSION_USERNAME] ON [Core].[Suspension](Username)
GO

CREATE INDEX [IX_SUSPENSION_CHANNELOFORIGIN] ON [Core].[Suspension](ChannelOfOrigin)
GO