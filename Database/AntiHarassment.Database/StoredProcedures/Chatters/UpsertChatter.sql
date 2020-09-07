CREATE PROCEDURE [Core].[UpsertChatter]
	@twitchUsername NVARCHAR(100),
	@firstTimeSeen DATETIME2(0)
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM [Core].[Chatters] WHERE [TwitchUsername] = @twitchUsername )
		INSERT INTO [Core].[Chatters]([TwitchUsername], [FirstTimeSeen])
		VALUES (@twitchUsername, @firstTimeSeen)
END