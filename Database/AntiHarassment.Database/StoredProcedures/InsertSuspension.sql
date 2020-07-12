CREATE PROCEDURE [Core].[InsertSuspension]
	@suspensionId UNIQUEIDENTIFIER,
	@username NVARCHAR(100),
	@channelOfOrigin NVARCHAR(100),
	@typeOfSuspension VARCHAR(100),
	@timestamp DATETIME2(0),
	@duration INT = 0,
	@data VARCHAR(MAX)
AS
BEGIN
	INSERT INTO [Core].[Suspension] ([SuspensionId], [Username], [ChannelOfOrigin], [TypeOfSuspension], [Timestamp], [Duration], [Data])
	VALUES (@suspensionId, @username, @channelOfOrigin, @typeOfSuspension, @timestamp, @duration, @data)
END