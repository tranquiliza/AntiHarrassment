CREATE PROCEDURE [Core].[InsertDeletedMessage]
	@channelOfOrigin NVARCHAR(100),
	@username NVARCHAR(100),
	@message NVARCHAR(2000),
	@timestamp DATETIME2(0),
	@deletedBy NVARCHAR(100)
AS
BEGIN
	INSERT INTO [Core].[DeletedMessages]([ChannelOfOrigin],[Username], [Message], [Timestamp], [DeletedBy])
	VALUES (@channelOfOrigin, @username, @message, @timestamp, @deletedBy)
END