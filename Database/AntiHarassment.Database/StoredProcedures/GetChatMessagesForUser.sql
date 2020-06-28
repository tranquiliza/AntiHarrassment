CREATE PROCEDURE [Core].[GetChatMessagesForUser]
	@username NVARCHAR(100),
	@channelOfOrigin NVARCHAR(100),
	@earliestTime DATETIME2(0)
AS
BEGIN
	SELECT [Message], [Timestamp] FROM [Core].[ChatMessage] 
	WHERE [Username] = @username 
	AND [ChannelOfOrigin] = @channelOfOrigin
	AND [Timestamp] > @earliestTime
END