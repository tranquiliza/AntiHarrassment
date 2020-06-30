CREATE PROCEDURE [Core].[UpsertChannel]
	@channelName NVARCHAR(100),
	@shouldListen bit
AS
BEGIN
	IF NOT EXISTS (SELECT * FROM [Core].[Channel] WHERE [ChannelName] = @channelName)
		INSERT INTO [Core].[Channel]([ChannelName], [ShouldListen]) VALUES (@channelName, @shouldListen)
	ELSE
		UPDATE [Core].[Channel]
		SET
		[ShouldListen] = @shouldListen
		WHERE [ChannelName] = @channelName
END
