CREATE PROCEDURE [Core].[GetChannel]
	@twitchUsername NVARCHAR(100)
AS
BEGIN
	SELECT TOP(1) [Data] FROM [Core].[Channel] WHERE [ChannelName] = @twitchUsername
END