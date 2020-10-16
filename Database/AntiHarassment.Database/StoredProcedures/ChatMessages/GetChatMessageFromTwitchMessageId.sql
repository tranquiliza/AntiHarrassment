CREATE PROCEDURE [Core].[GetChatMessageFromTwitchMessageId]
	@twitchMessageId NVARCHAR(MAX)
AS
BEGIN
	SELECT TOP(1) [Data] FROM [Core].[ChatMessage] 
	WHERE [TwitchMessageId] = @twitchMessageId
END