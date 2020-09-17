CREATE TABLE [Core].[DeletedMessages]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Username] NVARCHAR(100) NOT NULL, 
    [ChannelOfOrigin] NVARCHAR(100) NOT NULL, 
    [Message] NVARCHAR(2000) NOT NULL,
    [Timestamp] DATETIME2(0) NOT NULL, 
    [DeletedBy] NVARCHAR(100) NOT NULL 
)
