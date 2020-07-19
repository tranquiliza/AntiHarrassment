﻿CREATE TABLE [Core].[Users]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
    [TwitchUsername] NVARCHAR(1000) NOT NULL, 
    [Email] NVARCHAR(1000) NULL, 
    [Data] NVARCHAR(MAX) NOT NULL
)
GO

CREATE UNIQUE INDEX [IX_USERS_USERID] ON [Core].[Users]([UserId])
GO
CREATE INDEX [IX_USERS_TWITCHUSERNAME] ON [Core].[Users]([TwitchUsername])
GO
CREATE INDEX [IX_USERS_EMAIL] ON [Core].[Users]([Email])
GO