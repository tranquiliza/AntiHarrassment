CREATE PROCEDURE [Core].[GetUniqueChattersForChannel]
	@channelOfOrigin NVARCHAR(100)
AS
BEGIN
	SELECT DISTINCT([Username]) FROM [Core].[ChatMessage] WHERE [ChannelOfOrigin] = @channelOfOrigin
END