CREATE PROCEDURE [Core].[GetChatLogForChannel]
	@channelOfOrigin NVARCHAR(100),
	@earliestTime DATETIME2(0),
	@latestTime DATETIME2(0)
AS
BEGIN
	SELECT [Username], [Message], [Timestamp] FROM [Core].[ChatMessage] 
	WHERE [ChannelOfOrigin] = @channelOfOrigin
	AND [Timestamp] > @earliestTime
	AND [Timestamp] < @latestTime
END