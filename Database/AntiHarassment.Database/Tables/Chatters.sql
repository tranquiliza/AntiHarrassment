CREATE TABLE [Core].[Chatters]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [TwitchUsername] NVARCHAR(100) NOT NULL, 
    [FirstTimeSeen] DATETIME2(0) NOT NULL
)
