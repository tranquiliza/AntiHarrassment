CREATE PROCEDURE [Core].[InsertChatMessage]
	@username NVARCHAR(100),
	@channelOfOrigin NVARCHAR(100),
	@message NVARCHAR(2000),
	@timestamp DATETIME2(0)
AS
BEGIN
	INSERT INTO [Core].[ChatMessage]([Username], [ChannelOfOrigin], [Message],[Timestamp])
	VALUES (@username, @channelOfOrigin, @message, @timestamp)
END