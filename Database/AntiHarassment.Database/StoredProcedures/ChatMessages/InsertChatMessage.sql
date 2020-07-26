CREATE PROCEDURE [Core].[InsertChatMessage]
	@username NVARCHAR(100),
	@channelOfOrigin NVARCHAR(100),
	@automodded BIT,
	@message NVARCHAR(2000),
	@timestamp DATETIME2(0)
AS
BEGIN
	INSERT INTO [Core].[ChatMessage]([Username], [ChannelOfOrigin], [AutoModded], [Message],[Timestamp])
	VALUES (@username, @channelOfOrigin, @automodded, @message, @timestamp)
END