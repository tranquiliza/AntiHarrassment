CREATE PROCEDURE [Core].[InsertSuspension]
	@username NVARCHAR(100),
	@channelOfOrigin NVARCHAR(100),
	@typeOfSuspension VARCHAR(100),
	@timestamp DATETIME2(0),
	@duration INT = 0
AS
BEGIN
	INSERT INTO [Core].[Suspension] ([Username], [ChannelOfOrigin], [TypeOfSuspension], [Timestamp], [Duration])
	VALUES (@username, @channelOfOrigin, @typeOfSuspension, @timestamp, @duration)
END