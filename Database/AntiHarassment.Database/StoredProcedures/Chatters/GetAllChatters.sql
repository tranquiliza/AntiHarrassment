CREATE PROCEDURE [Core].[GetAllChatters]
AS
BEGIN
	SELECT [TwitchUsername] FROM [Chatters]
END