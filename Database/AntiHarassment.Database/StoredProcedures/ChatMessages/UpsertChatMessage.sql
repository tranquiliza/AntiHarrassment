CREATE PROCEDURE [Core].[UpsertChatMessage]
    @chatMessageId UNIQUEIDENTIFIER,
	@twitchMessageId NVARCHAR(MAX),
	@username NVARCHAR(100),
	@channelOfOrigin NVARCHAR(100),
	@automodded BIT,
	@deleted BIT,
	@message NVARCHAR(2000),
	@timestamp DATETIME2(0),
	@data NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM [Core].[ChatMessage] WHERE [ChatMessageId] = @chatMessageID)
		INSERT INTO [Core].[ChatMessage]([ChatMessageId], [TwitchMessageId], [Username], [ChannelOfOrigin], [Message], [Timestamp], [AutoModded], [Deleted], [Data])
		VALUES (@chatMessageID, @twitchMessageId, @username, @channelOfOrigin, @message, @timestamp, @automodded, @deleted, @data)
	ELSE
		UPDATE [Core].[ChatMessage] SET 
		[AutoModded] = @automodded,
		[Deleted] = @deleted,
		[Data] = @data
		WHERE [ChatMessageId] = @chatMessageId
END