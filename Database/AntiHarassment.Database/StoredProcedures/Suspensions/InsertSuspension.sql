CREATE PROCEDURE [Core].[InsertSuspension]
	@suspensionId UNIQUEIDENTIFIER,
	@username NVARCHAR(100),
	@channelOfOrigin NVARCHAR(100),
	@typeOfSuspension NVARCHAR(100),
	@timestamp DATETIME2(0),
	@duration INT = 0,
	@data NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM [Core].[Suspension] WHERE [SuspensionId] = @suspensionId)
		INSERT INTO [Core].[Suspension] ([SuspensionId], [Username], [ChannelOfOrigin], [TypeOfSuspension], [Timestamp], [Duration], [Data])
		VALUES (@suspensionId, @username, @channelOfOrigin, @typeOfSuspension, @timestamp, @duration, @data)
	ELSE
		UPDATE [Core].[Suspension] SET 
		[Username] = @username,
		[ChannelOfOrigin] = @channelOfOrigin,
		[TypeOfSuspension] = @typeOfSuspension,
		[Timestamp] = @timestamp,
		[Duration] = @duration,
		[Data] = @data
		WHERE [SuspensionId] = @suspensionId	
END