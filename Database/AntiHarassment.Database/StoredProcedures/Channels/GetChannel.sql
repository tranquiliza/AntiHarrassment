CREATE PROCEDURE [Core].[GetChannel]
	@twitchUsername NVARCHAR(100)
AS
BEGIN
	SELECT [Data] FROM [Core].[Channel] WHERE [ChannelName] = @twitchUsername
END