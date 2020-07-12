CREATE PROCEDURE [Core].[GetUserByTwitchUsername]
	@twitchUsername NVARCHAR(1000)
AS
BEGIN
	SELECT [Data] FROM [Core].[Users] WHERE [TwitchUsername] = @twitchUsername
END