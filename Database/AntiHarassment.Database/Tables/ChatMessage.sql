﻿CREATE TABLE [Core].[ChatMessage]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Username] NVARCHAR(100) NOT NULL, 
    [ChannelOfOrigin] NVARCHAR(100) NOT NULL,
    [Message] NVARCHAR(2000) NOT NULL, 
    [Timestamp] DATETIME2(0) NOT NULL
)
GO

CREATE INDEX [IX_CHATMESSAGE_USERNAME] ON [Core].[ChatMessage](Username)
GO

CREATE INDEX [IX_CHATMESSAGE_CHANNELOFORIGIN] ON [Core].[ChatMessage](ChannelOfOrigin)
GO