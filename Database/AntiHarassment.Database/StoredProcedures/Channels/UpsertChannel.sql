CREATE PROCEDURE [Core].[UpsertChannel]
	@channelId UNIQUEIDENTIFIER,
	@channelName NVARCHAR(100),
	@shouldListen bit,
	@data NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS (SELECT * FROM [Core].[Channel] WHERE [ChannelName] = @channelName)
		INSERT INTO [Core].[Channel]([ChannelId],[ChannelName],[ShouldListen],[Data])
		VALUES (@channelId, @channelName, @shouldListen, @data)
	ELSE
		UPDATE [Core].[Channel]
		SET
		[ShouldListen] = @shouldListen,
		[Data] = @data
		WHERE [ChannelName] = @channelName
END
